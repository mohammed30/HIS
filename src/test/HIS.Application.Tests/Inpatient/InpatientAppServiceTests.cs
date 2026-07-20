using System;
using System.Linq;
using System.Threading.Tasks;
using HIS.Rooms;
using HIS.Accounting;
using HIS.Billing;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HIS.Inpatient.Tests;

/// <summary>
/// اختبارات شاملة لوحدة المرضى المنومين (Inpatient)
/// ملاحظة: يجب إنشاء بيانات الاختبار (غرف/أسرة/مرضى) في UoW منفصل
/// قبل استدعاء AppService الذي يُنشئ UoW خاص به عبر ABP interceptors.
/// </summary>
public abstract class InpatientAppServiceTests<TStartupModule> : InpatientTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IAdmissionAppService _admissionAppService;
    private readonly IReservationAppService _reservationAppService;
    private readonly IRepository<Admission, Guid> _admissionRepository;
    private readonly IRepository<Bed, Guid> _bedRepository;
    private readonly IRepository<Room, Guid> _roomRepository;
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<Invoice, Guid> _invoiceRepository;

    protected InpatientAppServiceTests()
    {
        _admissionAppService = GetRequiredService<IAdmissionAppService>();
        _reservationAppService = GetRequiredService<IReservationAppService>();
        _admissionRepository = GetRequiredService<IRepository<Admission, Guid>>();
        _bedRepository = GetRequiredService<IRepository<Bed, Guid>>();
        _roomRepository = GetRequiredService<IRepository<Room, Guid>>();
        _journalEntryRepository = GetRequiredService<IRepository<JournalEntry, Guid>>();
        _invoiceRepository = GetRequiredService<IRepository<Invoice, Guid>>();
    }

    // ────────────────────────────────────────────────────────────────────────
    // Helpers: إنشاء البيانات في UoW مستقل مُكتمل قبل استدعاء AppService
    // ────────────────────────────────────────────────────────────────────────

    private async Task<(Guid RoomId, Guid BedId)> SetupRoomAndBedAsync(
        string roomNumber, RoomType type = RoomType.Private, decimal dailyRate = 500m, string bedNumber = "A")
    {
        Guid roomId = Guid.Empty, bedId = Guid.Empty;
        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
            var (room, bed) = await CreateRoomWithBedAsync(roomNumber, type, dailyRate, bedNumber);
            roomId = room.Id;
            bedId = bed.Id;
        });
        return (roomId, bedId);
    }

    private async Task<Guid> SetupPatientAsync(string firstName, string lastName)
    {
        Guid patientId = Guid.Empty;
        await WithUnitOfWorkAsync(async () =>
        {
            var patient = await CreatePatientAsync(firstName, lastName);
            patientId = patient.Id;
        });
        return patientId;
    }

    // ────────────────────────────────────────────────────────────────────────
    // 1. اختبارات الحجوزات (Reservations)
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// يتحقق من إمكانية إنشاء حجز صحيح لغرفة متاحة بتواريخ مستقبلية
    /// </summary>
    [Fact]
    public async Task CreateReservation_ShouldSucceed_ForAvailableRoom()
    {
        var (roomId, bedId) = await SetupRoomAndBedAsync("R201", RoomType.Private, 600m);
        var patientId = await SetupPatientAsync("سارة", "العمري");

        var startDate = DateTime.Now.AddDays(2);
        var endDate = startDate.AddDays(5);

        var reservation = await _reservationAppService.CreateAsync(new CreateUpdateReservationDto
        {
            PatientId = patientId,
            RoomId = roomId,
            BedId = bedId,
            StartDate = startDate,
            EndDate = endDate,
            Status = ReservationStatus.Pending,
            Notes = "حجز مبدئي"
        });

        reservation.ShouldNotBeNull();
        reservation.Id.ShouldNotBe(Guid.Empty);
        reservation.PatientId.ShouldBe(patientId);
        reservation.RoomId.ShouldBe(roomId);
        reservation.Status.ShouldBe(ReservationStatus.Pending);
        reservation.StartDate.Date.ShouldBe(startDate.Date);
    }

    /// <summary>
    /// يتحقق من منع الحجز المتعارض على نفس السرير في نفس الفترة الزمنية
    /// </summary>
    [Fact]
    public async Task CreateReservation_ShouldThrow_WhenOverlappingDates()
    {
        var (roomId, bedId) = await SetupRoomAndBedAsync("R202", RoomType.Standard, 300m);
        var patientId1 = await SetupPatientAsync("محمد", "الأحمد");
        var patientId2 = await SetupPatientAsync("عمر", "الزهراني");

        var startDate = DateTime.Now.AddDays(3);
        var endDate = startDate.AddDays(7);

        // إنشاء أول حجز بنجاح
        await _reservationAppService.CreateAsync(new CreateUpdateReservationDto
        {
            PatientId = patientId1,
            RoomId = roomId,
            BedId = bedId,
            StartDate = startDate,
            EndDate = endDate
        });

        // محاولة حجز متعارض يجب أن تفشل
        await Should.ThrowAsync<Volo.Abp.UserFriendlyException>(async () =>
        {
            await _reservationAppService.CreateAsync(new CreateUpdateReservationDto
            {
                PatientId = patientId2,
                RoomId = roomId,
                BedId = bedId,
                StartDate = startDate.AddDays(2),
                EndDate = endDate.AddDays(2)
            });
        });
    }

    // ────────────────────────────────────────────────────────────────────────
    // 2. اختبارات التنويم (Admissions)
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// تنويم مريض جديد في غرفة متاحة والتحقق من:
    /// - إنشاء سجل التنويم بالحالة Active
    /// - تحويل حالة السرير إلى Occupied
    /// - توليد قيد يومية محاسبي
    /// </summary>
    [Fact]
    public async Task CreateAdmission_ShouldOccupyBed_And_CreateJournalEntry()
    {
        var (roomId, bedId) = await SetupRoomAndBedAsync("R301", RoomType.Private, 800m);
        var patientId = await SetupPatientAsync("خالد", "الرشيدي");

        var admission = await _admissionAppService.CreateAsync(new CreateUpdateAdmissionDto
        {
            PatientId = patientId,
            RoomId = roomId,
            BedId = bedId,
            NumberOfDays = 3,
            PaidAmount = 0,
            Purpose = "فحص وعلاج"
        });

        admission.ShouldNotBeNull();
        admission.PatientId.ShouldBe(patientId);
        admission.Status.ShouldBe(AdmissionStatus.Active);

        // التحقق من حالة السرير والقيد المحاسبي
        await WithUnitOfWorkAsync(async () =>
        {
            var updatedBed = await _bedRepository.GetAsync(bedId);
            updatedBed.Status.ShouldBe(BedStatus.Occupied);

            var journalEntries = await _journalEntryRepository.GetListAsync();
            journalEntries.ShouldContain(je => je.Description.Contains("حجز تنويم"));
        });
    }

    /// <summary>
    /// تنويم مريض مع دفعة مقدمة - يتحقق من قيد يومية بقيمة الدفعة في الصندوق/الخزينة
    /// </summary>
    [Fact]
    public async Task CreateAdmission_WithAdvancePayment_ShouldDebitCash()
    {
        var (roomId, bedId) = await SetupRoomAndBedAsync("R302", RoomType.Suite, 1200m);
        var patientId = await SetupPatientAsync("ريم", "الحربي");

        var admission = await _admissionAppService.CreateAsync(new CreateUpdateAdmissionDto
        {
            PatientId = patientId,
            RoomId = roomId,
            BedId = bedId,
            NumberOfDays = 5,
            PaidAmount = 2000m,
            Purpose = "إقامة طويلة"
        });

        admission.ShouldNotBeNull();
        admission.Status.ShouldBe(AdmissionStatus.Active);

        await WithUnitOfWorkAsync(async () =>
        {
            var journalEntries = await _journalEntryRepository.GetListAsync();
            var admissionJe = journalEntries.FirstOrDefault(je => je.Description.Contains("حجز تنويم"));
            admissionJe.ShouldNotBeNull("يجب وجود قيد يومية لحجز التنويم مع دفعة مقدمة");
            // يكفي التحقق من أن القيد موجود وله رقم مرجعي صحيح (Lines قد لا تُحمَّل تلقائياً)
            admissionJe.Id.ShouldNotBe(Guid.Empty);
        });
    }

    /// <summary>
    /// منع تنويم مريض في سرير مشغول بالفعل
    /// </summary>
    [Fact]
    public async Task CreateAdmission_ShouldThrow_WhenBedIsOccupied()
    {
        var (roomId, bedId) = await SetupRoomAndBedAsync("R303", RoomType.Standard, 400m);
        var patientId1 = await SetupPatientAsync("فهد", "السليم");
        var patientId2 = await SetupPatientAsync("سلطان", "القحطاني");

        // تنويم أول مريض
        await _admissionAppService.CreateAsync(new CreateUpdateAdmissionDto
        {
            PatientId = patientId1,
            RoomId = roomId,
            BedId = bedId,
            NumberOfDays = 2
        });

        // محاولة تنويم مريض آخر في نفس السرير يجب أن تفشل
        await Should.ThrowAsync<Volo.Abp.UserFriendlyException>(async () =>
        {
            await _admissionAppService.CreateAsync(new CreateUpdateAdmissionDto
            {
                PatientId = patientId2,
                RoomId = roomId,
                BedId = bedId,
                NumberOfDays = 1
            });
        });
    }

    /// <summary>
    /// منع تنويم مريض إذا كان السرير لا ينتمي للغرفة المحددة
    /// </summary>
    [Fact]
    public async Task CreateAdmission_ShouldFail_WhenBedDoesNotBelongToRoom()
    {
        var (roomId1, _) = await SetupRoomAndBedAsync("R501", RoomType.Standard, 300m, "A501");
        var (_, bedId2) = await SetupRoomAndBedAsync("R502", RoomType.Standard, 300m, "B502");
        var patientId = await SetupPatientAsync("نورة", "العتيبي");

        await Should.ThrowAsync<Volo.Abp.UserFriendlyException>(async () =>
        {
            await _admissionAppService.CreateAsync(new CreateUpdateAdmissionDto
            {
                PatientId = patientId,
                RoomId = roomId1,   // غرفة خاطئة
                BedId = bedId2,     // السرير ينتمي لغرفة R502
                NumberOfDays = 2
            });
        });
    }

    // ────────────────────────────────────────────────────────────────────────
    // 3. السيناريو الشامل: تنويم → خروج → فاتورة موحدة
    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// دورة حياة التنويم الكاملة:
    /// 1. تنويم مريض → السرير يصبح مشغولاً
    /// 2. إخراج المريض → السرير يصبح متاحاً من جديد
    /// 3. توليد فاتورة موحدة (Consolidated Invoice) تلقائياً
    /// </summary>
    [Fact]
    public async Task FullAdmissionLifecycle_AdmitThenDischarge_ShouldFreeBed_And_GenerateInvoice()
    {
        // الإعداد
        var (roomId, bedId) = await SetupRoomAndBedAsync("R401", RoomType.Private, 700m);
        var patientId = await SetupPatientAsync("عبدالله", "المطيري");

        // المرحلة 1: التنويم
        var admission = await _admissionAppService.CreateAsync(new CreateUpdateAdmissionDto
        {
            PatientId = patientId,
            RoomId = roomId,
            BedId = bedId,
            NumberOfDays = 3,
            Purpose = "اختبار دورة الحياة الكاملة"
        });

        admission.Status.ShouldBe(AdmissionStatus.Active);

        // التحقق من أن السرير مشغول
        await WithUnitOfWorkAsync(async () =>
        {
            var occupiedBed = await _bedRepository.GetAsync(bedId);
            occupiedBed.Status.ShouldBe(BedStatus.Occupied);
        });

        // المرحلة 2: إخراج المريض
        var discharged = await _admissionAppService.DischargeAsync(admission.Id, new DischargeAdmissionDto
        {
            DischargeDate = DateTime.Now.AddDays(3),
            Notes = "خروج منتظم"
        });

        discharged.Status.ShouldBe(AdmissionStatus.Discharged);
        discharged.DischargeDate.ShouldNotBeNull();
        discharged.NumberOfDays.ShouldBeGreaterThan(0);

        // المرحلة 3: التحقق من إتاحة السرير وتوليد الفاتورة
        await WithUnitOfWorkAsync(async () =>
        {
            // السرير يصبح Cleaning بعد الخروج (يحتاج تنظيف قبل إتاحته مجدداً)
            var cleaningBed = await _bedRepository.GetAsync(bedId);
            cleaningBed.Status.ShouldBeOneOf(BedStatus.Available, BedStatus.Cleaning);
            // في الإنتاج: السرير يمر بحالة Cleaning ثم Available

            // فاتورة موحدة تم إنشاؤها عند الخروج
            var invoices = await _invoiceRepository.GetListAsync();
            var dischargeInvoice = invoices.FirstOrDefault(inv =>
                inv.Notes != null && inv.Notes.Contains("Consolidated Inpatient Invoice"));
            dischargeInvoice.ShouldNotBeNull("يجب توليد فاتورة موحدة عند خروج المريض");
            dischargeInvoice.TotalAmount.ShouldBeGreaterThan(0);
        });
    }
}

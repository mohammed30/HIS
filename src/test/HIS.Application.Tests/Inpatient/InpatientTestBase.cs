using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIS.Accounting;
using HIS.Rooms;
using HIS.Patients;
using Volo.Abp.Domain.Repositories;

namespace HIS.Inpatient.Tests;

/// <summary>
/// قاعدة الاختبار لوحدة المرضى المنومين - تجهيز الغرف والأسرة والمرضى والحسابات
/// </summary>
public abstract class InpatientTestBase<TStartupModule> : HISApplicationTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    protected readonly IRepository<Account, Guid> AccountRepository;
    protected readonly IRepository<AccountMapping, Guid> AccountMappingRepository;
    protected readonly IRepository<Room, Guid> RoomRepository;
    protected readonly IRepository<Bed, Guid> BedRepository;
    protected readonly IRepository<Patient, Guid> PatientRepository;

    protected InpatientTestBase()
    {
        AccountRepository = GetRequiredService<IRepository<Account, Guid>>();
        AccountMappingRepository = GetRequiredService<IRepository<AccountMapping, Guid>>();
        RoomRepository = GetRequiredService<IRepository<Room, Guid>>();
        BedRepository = GetRequiredService<IRepository<Bed, Guid>>();
        PatientRepository = GetRequiredService<IRepository<Patient, Guid>>();
    }

    /// <summary>
    /// إعداد الحسابات المحاسبية المطلوبة لإنشاء القيود التلقائية
    /// </summary>
    protected virtual async Task EnsureAccountMappingsAreFilledAsync()
    {
        var required = new[]
        {
            (Code: "4100", Name: "Medical Services Revenue", NameAr: "إيرادات الخدمات الطبية", Type: AccountType.Revenue,   Map: AccountMappingType.SalesRevenue),
            (Code: "1110", Name: "Cash",                     NameAr: "الخزينة",                  Type: AccountType.Asset,     Map: AccountMappingType.CashAccount),
            (Code: "1111", Name: "Bank",                     NameAr: "البنك",                    Type: AccountType.Asset,     Map: AccountMappingType.CardPaymentBank),
            (Code: "2200", Name: "VAT Output",               NameAr: "ضريبة مخرجات",             Type: AccountType.Liability, Map: AccountMappingType.VATOutput),
            (Code: "1120", Name: "Accounts Receivable",      NameAr: "ذمم مدينة",                Type: AccountType.Asset,     Map: AccountMappingType.VATInput),
            (Code: "1121", Name: "Patients Receivable",      NameAr: "ذمم مرضى",                 Type: AccountType.Asset,     Map: AccountMappingType.PatientsReceivable),
            (Code: "1122", Name: "Insurance Receivable",     NameAr: "ذمم تأمين",                Type: AccountType.Asset,     Map: AccountMappingType.InsuranceReceivable),
            (Code: "1130", Name: "Inventory",                NameAr: "المخزون",                  Type: AccountType.Asset,     Map: AccountMappingType.Inventory),
            (Code: "5200", Name: "COGS",                     NameAr: "تكلفة المبيعات",           Type: AccountType.Expense,   Map: AccountMappingType.COGS),
        };

        var accountCache = new Dictionary<string, Guid>();
        foreach (var r in required)
        {
            if (!accountCache.ContainsKey(r.Code))
            {
                var existing = await AccountRepository.FirstOrDefaultAsync(x => x.Code == r.Code);
                if (existing == null)
                {
                    var acc = new Account(Guid.NewGuid(), r.Code, r.Name, r.NameAr, r.Type);
                    await AccountRepository.InsertAsync(acc);
                    accountCache[r.Code] = acc.Id;
                }
                else
                {
                    accountCache[r.Code] = existing.Id;
                }
            }

            var accountId = accountCache[r.Code];
            var mapping = await AccountMappingRepository.FirstOrDefaultAsync(x => x.MappingType == r.Map);
            if (mapping == null)
            {
                await AccountMappingRepository.InsertAsync(
                    new AccountMapping(Guid.NewGuid(), r.Map, accountId, isMandatory: true));
            }
            else if (mapping.AccountId == null)
            {
                mapping.AccountId = accountId;
                await AccountMappingRepository.UpdateAsync(mapping);
            }
        }
    }

    /// <summary>
    /// إنشاء غرفة وسرير للاستخدام في الاختبارات
    /// </summary>
    protected virtual async Task<(Room Room, Bed Bed)> CreateRoomWithBedAsync(
        string roomNumber = "101",
        RoomType type = RoomType.Private,
        decimal dailyRate = 500m,
        string bedNumber = "A")
    {
        var room = new Room(Guid.NewGuid(), null, roomNumber, type, dailyRate, bedCount: 1);
        await RoomRepository.InsertAsync(room);

        var bed = new Bed(Guid.NewGuid(), null, room.Id, bedNumber, BedType.Standard, BedStatus.Available);
        await BedRepository.InsertAsync(bed);

        return (room, bed);
    }

    /// <summary>
    /// إنشاء مريض بسيط للاستخدام في الاختبارات
    /// </summary>
    protected virtual async Task<Patient> CreatePatientAsync(string firstName = "أحمد", string lastName = "محمد")
    {
        var existing = await PatientRepository.FirstOrDefaultAsync(x => x.FirstNameAr == firstName && x.LastNameAr == lastName);
        if (existing != null) return existing;

        var mrn = $"MRN-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        var patient = new Patient(
            Guid.NewGuid(),
            null,
            mrn,
            firstName,
            lastName,
            null,
            HIS.Patients.Gender.Male,
            HIS.Patients.IdentityType.NationalId,
            "0000000000",
            "0500000000");
        await PatientRepository.InsertAsync(patient);
        return patient;
    }
}

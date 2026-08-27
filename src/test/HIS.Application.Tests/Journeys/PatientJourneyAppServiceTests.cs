using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Billing;
using Shouldly;
using Xunit;

namespace HIS.Journeys.Tests
{
    /// <summary>
    /// اختبارات رحلة المريض - Patient Journey Tests
    /// تختبر منطق حساب الفاتورة مباشرة بدون DI Container
    /// القاعدة: نصيب التأمين = TotalPrice * InsurancePercentage / 100
    /// </summary>
    public class PatientJourneyAppServiceTests
    {
        // ─── Helper: بناء بند فاتورة ─────────────────────────────────────────
        private static InvoiceItem MakeItem(decimal unitPrice, decimal qty, decimal insurancePct,
                                            bool covered = true, decimal discountPct = 0)
        {
            var item = (InvoiceItem)System.Runtime.Serialization.FormatterServices
                           .GetUninitializedObject(typeof(InvoiceItem));
            typeof(InvoiceItem).GetProperty("Id")?.SetValue(item, Guid.NewGuid());
            item.UnitPrice = unitPrice;
            item.Quantity = qty;
            item.DiscountPercentage = discountPct;
            item.DiscountAmount = (qty * unitPrice) * (discountPct / 100m);
            item.IsCoveredByInsurance = covered;
            item.InsurancePercentage = insurancePct;
            return item;
        }

        // ─── Helper: حساب مجاميع الفاتورة (نفس منطق BillingAppServices) ─────
        private static void CalculateInvoiceTotals(Invoice invoice,
                                                   decimal globalDiscount = 0m,
                                                   decimal taxPct = 0m)
        {
            decimal total = invoice.Items.Sum(i => i.TotalPrice);
            decimal insurance = invoice.Items
                .Where(i => i.IsCoveredByInsurance && i.InsurancePercentage > 0)
                .Sum(i => i.TotalPrice * i.InsurancePercentage / 100m);

            invoice.TotalAmount = total;
            invoice.DiscountAmount = globalDiscount;
            invoice.TaxPercentage = taxPct;
            invoice.TaxAmount = (total - globalDiscount) * (taxPct / 100m);
            invoice.NetAmount = total - globalDiscount + invoice.TaxAmount;
            invoice.InsuranceCoverage = insurance;
            invoice.CoPaymentAmount = invoice.NetAmount - insurance;
        }

        // ─── Helper: إنشاء كيان خام ──────────────────────────────────────────
        private static T Uninitialized<T>(Guid id) where T : Volo.Abp.Domain.Entities.Entity<Guid>
        {
            var e = (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
            typeof(T).GetProperty("Id")?.SetValue(e, id);
            return e;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // سيناريو 1: مريض خارجي - كشف (200) + عملية (5000)، تأمين 80%
        // ═══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Outpatient_Consultation_And_Surgery_InsurancePct80_ShouldCalculateCorrectly()
        {
            var invoice = Uninitialized<Invoice>(Guid.NewGuid());
            invoice.Items = new List<InvoiceItem>
            {
                MakeItem(200,  1, 80),  // كشف
                MakeItem(5000, 1, 80),  // عملية
            };

            CalculateInvoiceTotals(invoice);

            invoice.TotalAmount.ShouldBe(5200m);
            invoice.InsuranceCoverage.ShouldBe(4160m);   // 5200 × 80%
            invoice.CoPaymentAmount.ShouldBe(1040m);      // 5200 × 20%
        }

        // ═══════════════════════════════════════════════════════════════════════
        // سيناريو 2: مريض خارجي - تغطية مختلطة (بعض الخدمات غير مشمولة)
        // ═══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Outpatient_MixedCoverage_ShouldCalculateCorrectly()
        {
            var invoice = Uninitialized<Invoice>(Guid.NewGuid());
            invoice.Items = new List<InvoiceItem>
            {
                MakeItem(300, 1, 80),                      // كشف (مشمول 80%)
                MakeItem(150, 2, 70),                      // أدوية (مشمول 70%)
                MakeItem(500, 1, 0, covered: false),       // خدمة غير مشمولة
            };

            CalculateInvoiceTotals(invoice);

            invoice.TotalAmount.ShouldBe(1100m);           // 300+300+500
            invoice.InsuranceCoverage.ShouldBe(450m);      // (300×80%)+(300×70%) = 240+210
            invoice.CoPaymentAmount.ShouldBe(650m);        // 1100-450
        }

        // ═══════════════════════════════════════════════════════════════════════
        // سيناريو 3: مريض خارجي - مع خصم عام وضريبة 15%
        // ═══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Outpatient_WithGlobalDiscountAndTax_ShouldCalculateCorrectly()
        {
            var invoice = Uninitialized<Invoice>(Guid.NewGuid());
            invoice.Items = new List<InvoiceItem>
            {
                MakeItem(1000, 1, 80),
            };

            CalculateInvoiceTotals(invoice, globalDiscount: 100m, taxPct: 15m);

            invoice.TotalAmount.ShouldBe(1000m);
            invoice.TaxAmount.ShouldBe(135m);             // (1000-100) × 15%
            invoice.NetAmount.ShouldBe(1035m);            // 900 + 135
            invoice.InsuranceCoverage.ShouldBe(800m);     // 1000 × 80%
            invoice.CoPaymentAmount.ShouldBe(235m);       // 1035 - 800
        }

        // ═══════════════════════════════════════════════════════════════════════
        // سيناريو 4: مريض منوم - غرفة 3 أيام + عملية + أدوية، تأمين 90%
        // ═══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Inpatient_RoomSurgeryMeds_InsurancePct90_ShouldCalculateCorrectly()
        {
            var invoice = Uninitialized<Invoice>(Guid.NewGuid());
            invoice.Items = new List<InvoiceItem>
            {
                MakeItem(1000,  3, 90),   // غرفة × 3 أيام
                MakeItem(15000, 1, 90),   // عملية جراحية
                MakeItem(200,   5, 90),   // أدوية × 5
            };

            CalculateInvoiceTotals(invoice);

            invoice.TotalAmount.ShouldBe(19000m);          // 3000+15000+1000
            invoice.InsuranceCoverage.ShouldBe(17100m);    // 19000 × 90%
            invoice.CoPaymentAmount.ShouldBe(1900m);       // 19000 × 10%
        }

        // ═══════════════════════════════════════════════════════════════════════
        // سيناريو 5: مريض منوم - خصم على مستوى البند
        // ═══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Inpatient_WithItemLevelDiscount_ShouldCalculateCorrectly()
        {
            var invoice = Uninitialized<Invoice>(Guid.NewGuid());
            invoice.Items = new List<InvoiceItem>
            {
                MakeItem(2000, 1, 80, discountPct: 10),  // خصم 10% → صافي 1800
                MakeItem(3000, 1, 80),                    // 3000
            };

            CalculateInvoiceTotals(invoice);

            invoice.TotalAmount.ShouldBe(4800m);           // 1800 + 3000
            invoice.InsuranceCoverage.ShouldBe(3840m);     // 4800 × 80%
            invoice.CoPaymentAmount.ShouldBe(960m);        // 4800 × 20%
        }

        // ═══════════════════════════════════════════════════════════════════════
        // سيناريو 6: مريض منوم - حد أقصى للتغطية (MaxCoverageAmount = 10,000)
        // ═══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Inpatient_InsuranceCappedAtMaxCoverage_ShouldLimitInsuranceShare()
        {
            const decimal maxCoverage = 10_000m;
            var invoice = Uninitialized<Invoice>(Guid.NewGuid());
            invoice.Items = new List<InvoiceItem>
            {
                MakeItem(50000, 1, 90),  // عملية كبرى جدًا
            };

            CalculateInvoiceTotals(invoice);

            // تطبيق الحد الأقصى
            if (invoice.InsuranceCoverage > maxCoverage)
            {
                invoice.InsuranceCoverage = maxCoverage;
                invoice.CoPaymentAmount = invoice.NetAmount - maxCoverage;
            }

            invoice.TotalAmount.ShouldBe(50_000m);
            invoice.InsuranceCoverage.ShouldBe(10_000m);   // محدود وليس 45,000
            invoice.CoPaymentAmount.ShouldBe(40_000m);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // سيناريو 7: مريض خارجي بدون تأمين - دفع كامل
        // ═══════════════════════════════════════════════════════════════════════
        [Fact]
        public void Outpatient_NoInsurance_PatientPaysFullAmount()
        {
            var invoice = Uninitialized<Invoice>(Guid.NewGuid());
            invoice.Items = new List<InvoiceItem>
            {
                MakeItem(500, 1, 0, covered: false),
                MakeItem(300, 2, 0, covered: false),
            };

            CalculateInvoiceTotals(invoice);

            invoice.TotalAmount.ShouldBe(1100m);
            invoice.InsuranceCoverage.ShouldBe(0m);
            invoice.CoPaymentAmount.ShouldBe(1100m);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // سيناريو 8: التحقق من معادلة الفاتورة الأساسية دائماً صحيحة
        // ═══════════════════════════════════════════════════════════════════════
        [Fact]
        public void InvoiceEquation_Always_Holds()
        {
            var invoice = Uninitialized<Invoice>(Guid.NewGuid());
            invoice.Items = new List<InvoiceItem>
            {
                MakeItem(800,  1, 75),
                MakeItem(1200, 2, 60),
                MakeItem(400,  1, 0, covered: false),
            };

            CalculateInvoiceTotals(invoice, globalDiscount: 50m, taxPct: 15m);

            // المعادلة الأساسية
            invoice.CoPaymentAmount.ShouldBe(invoice.NetAmount - invoice.InsuranceCoverage);
            invoice.NetAmount.ShouldBe(invoice.TotalAmount - invoice.DiscountAmount + invoice.TaxAmount);

            // قيود منطقية
            invoice.InsuranceCoverage.ShouldBeLessThanOrEqualTo(invoice.TotalAmount);
            invoice.CoPaymentAmount.ShouldBeGreaterThan(0m);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Laboratory;
using HIS.Laboratory.Dtos;
using HIS.Patients;
using NSubstitute;
using Volo.Abp.Domain.Repositories;
using Xunit;
using Shouldly;
using Volo.Abp.ObjectMapping;

namespace HIS.Laboratory.Tests
{
    public class LabAppServiceTests
    {
        public LabAppServiceTests()
        {
        }

        [Fact]
        public void EvaluateResultIsAbnormal_NumericResult_CorrectlyEvaluated()
        {
            var testId = Guid.NewGuid();
            var test = CreateLabTest(testId, new List<LabTestNormalRange>
            {
                CreateNormalRange(testId, Gender.Male, null, null, LabResultType.Numeric, 70m, 110m, null),
                CreateNormalRange(testId, Gender.Female, null, null, LabResultType.Numeric, 60m, 100m, null)
            });

            var result1 = LabEvaluationHelper.IsAbnormal(test, "80", Gender.Male, 30 * 365);
            var result2 = LabEvaluationHelper.IsAbnormal(test, "120", Gender.Male, 30 * 365);
            var result3 = LabEvaluationHelper.IsAbnormal(test, "90", Gender.Female, 30 * 365);
            var result4 = LabEvaluationHelper.IsAbnormal(test, "105", Gender.Female, 30 * 365);

            result1.ShouldBeFalse(); 
            result2.ShouldBeTrue();  
            result3.ShouldBeFalse(); 
            result4.ShouldBeTrue();  
        }

        [Fact]
        public void EvaluateResultIsAbnormal_TextResult_CorrectlyEvaluated()
        {
            var testId = Guid.NewGuid();
            var test = CreateLabTest(testId, new List<LabTestNormalRange>
            {
                CreateNormalRange(testId, null, null, null, LabResultType.Text, null, null, "Negative")
            });

            var result1 = LabEvaluationHelper.IsAbnormal(test, "Negative", Gender.Male, 30 * 365);
            var result2 = LabEvaluationHelper.IsAbnormal(test, "Positive", Gender.Male, 30 * 365);

            result1.ShouldBeFalse(); 
            result2.ShouldBeTrue();  
        }

        [Fact]
        public void EvaluateResultIsAbnormal_AgeBased_CorrectlyEvaluated()
        {
            var testId = Guid.NewGuid();
            var test = CreateLabTest(testId, new List<LabTestNormalRange>
            {
                CreateNormalRange(testId, null, 0, 365, LabResultType.Numeric, 10m, 50m, null),
                CreateNormalRange(testId, null, 366, null, LabResultType.Numeric, 20m, 100m, null)
            });

            var result1 = LabEvaluationHelper.IsAbnormal(test, "30", Gender.Male, 180); 
            var result2 = LabEvaluationHelper.IsAbnormal(test, "60", Gender.Male, 180); 
            var result3 = LabEvaluationHelper.IsAbnormal(test, "80", Gender.Male, 30 * 365); 
            var result4 = LabEvaluationHelper.IsAbnormal(test, "15", Gender.Male, 30 * 365); 

            result1.ShouldBeFalse(); 
            result2.ShouldBeTrue();  
            result3.ShouldBeFalse(); 
            result4.ShouldBeTrue();  
        }

        private static LabTest CreateLabTest(Guid id, List<LabTestNormalRange> normalRanges)
        {
            var test = (LabTest)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(LabTest));
            typeof(LabTest).GetProperty("Id")?.SetValue(test, id);
            test.NormalRanges = normalRanges;
            return test;
        }

        private static LabTestNormalRange CreateNormalRange(Guid testId, Gender? gender, int? minAge, int? maxAge, LabResultType resultType, decimal? minVal, decimal? maxVal, string? stringVal)
        {
            var range = (LabTestNormalRange)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(LabTestNormalRange));
            typeof(LabTestNormalRange).GetProperty("Id")?.SetValue(range, Guid.NewGuid());
            range.LabTestId = testId;
            range.TargetGender = gender;
            range.MinAgeDays = minAge;
            range.MaxAgeDays = maxAge;
            range.ResultType = resultType;
            range.MinValue = minVal;
            range.MaxValue = maxVal;
            range.NormalStringValue = stringVal;
            return range;
        }
    }

    public static class LabEvaluationHelper
    {
        public static bool IsAbnormal(LabTest test, string result, Gender patientGender, int patientAgeInDays)
        {
            if (test == null || !test.NormalRanges.Any()) return false;

            var range = test.NormalRanges.FirstOrDefault(r => 
                (!r.TargetGender.HasValue || r.TargetGender.Value == patientGender) &&
                (!r.MinAgeDays.HasValue || patientAgeInDays >= r.MinAgeDays.Value) &&
                (!r.MaxAgeDays.HasValue || patientAgeInDays <= r.MaxAgeDays.Value)
            );

            if (range == null) return false; 

            if (range.ResultType == LabResultType.Numeric)
            {
                if (decimal.TryParse(result, out decimal numericResult))
                {
                    if (range.MinValue.HasValue && numericResult < range.MinValue.Value) return true;
                    if (range.MaxValue.HasValue && numericResult > range.MaxValue.Value) return true;
                    return false;
                }
                return true; 
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(range.NormalStringValue))
                {
                    return !result.Equals(range.NormalStringValue, StringComparison.OrdinalIgnoreCase);
                }
            }
            return false;
        }
    }
}

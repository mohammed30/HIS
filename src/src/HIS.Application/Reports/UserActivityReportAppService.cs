using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.ActivityLogs;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace HIS.Reports
{
    public class UserActivityReportAppService : ApplicationService, IUserActivityReportAppService
    {
        private readonly IRepository<ActivityLog, Guid> _activityLogRepository;
        private readonly IIdentityUserRepository _userRepository;

        public UserActivityReportAppService(
            IRepository<ActivityLog, Guid> activityLogRepository,
            IIdentityUserRepository userRepository)
        {
            _activityLogRepository = activityLogRepository;
            _userRepository = userRepository;
        }

        public async Task<PagedResultDto<UserActivityFrequencyDto>> GetListAsync(GetUserActivityFrequencyInput input)
        {
            if (input.StartDate.HasValue)
            {
                input.StartDate = input.StartDate.Value.Date;
            }
            if (input.EndDate.HasValue)
            {
                input.EndDate = input.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            }

            var query = await _activityLogRepository.GetQueryableAsync();

            query = query
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Module), x => x.Module == input.Module)
                .WhereIf(input.StartDate.HasValue, x => x.Timestamp >= input.StartDate)
                .WhereIf(input.EndDate.HasValue, x => x.Timestamp <= input.EndDate);

            // Fetch to memory for reliable grouping
            var logs = await AsyncExecuter.ToListAsync(query);

            var grouped = logs
                .GroupBy(x => new { x.UserId, x.Module, x.EntityType, x.Action, Date = x.Timestamp.Date })
                .Select(g => new UserActivityFrequencyDto
                {
                    UserId = g.Key.UserId,
                    Module = g.Key.Module,
                    EntityType = string.IsNullOrWhiteSpace(g.Key.EntityType) ? "شاشة رئيسية / أخرى" : g.Key.EntityType,
                    Action = GetActionName(g.Key.Action),
                    Date = g.Key.Date,
                    LastAccessTime = g.Max(x => x.Timestamp),
                    FrequencyCount = g.Count()
                })
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.LastAccessTime)
                .ToList();

            // Map user names
            var userIds = grouped.Where(x => x.UserId.HasValue).Select(x => x.UserId.Value).Distinct().ToList();
            var users = await _userRepository.GetListByIdsAsync(userIds);
            var userDict = users.ToDictionary(x => x.Id, x => x.Name ?? x.UserName);

            foreach (var item in grouped)
            {
                if (item.UserId.HasValue && userDict.TryGetValue(item.UserId.Value, out var userName))
                {
                    item.UserName = userName;
                }
                else
                {
                    item.UserName = "النظام / غير معروف";
                }
            }

            var paginated = grouped
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();

            return new PagedResultDto<UserActivityFrequencyDto>(grouped.Count, paginated);
        }

        private string GetActionName(ActivityAction action)
        {
            return action switch
            {
                ActivityAction.Login => "تسجيل الدخول",
                ActivityAction.Logout => "تسجيل الخروج",
                ActivityAction.Create => "إضافة/إنشاء",
                ActivityAction.Update => "تعديل",
                ActivityAction.Delete => "حذف",
                ActivityAction.View => "عرض/فتح الشاشة",
                ActivityAction.Export => "تصدير",
                ActivityAction.Import => "استيراد",
                ActivityAction.AccessDenied => "محاولة دخول مرفوضة",
                ActivityAction.FailedLogin => "فشل تسجيل الدخول",
                _ => action.ToString()
            };
        }
    }
}

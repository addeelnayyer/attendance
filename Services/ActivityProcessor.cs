using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;

namespace Aquila360.Attendance.Services
{
    public class ActivityProcessor : IActivityProcessor
    {
        public IEnumerable<AttendanceModel> ProcessDailyActivityResponse(HubStaffDailyActivityResponse response)
        {
            var users = response.Users.ToDictionary(x => x.Id);
            var projects = response.Projects.ToDictionary(x => x.Id);
            var tasks = response.Tasks.ToDictionary(x => x.Id);

            var result = new List<AttendanceModel>();

            foreach (var dailyActivity in response.DailyActivities)
            {
                result.Add(new AttendanceModel
                {
                    Id = dailyActivity.Id.ToString(),
                    Email = users.ContainsKey(dailyActivity.UserId)
                        ? users[dailyActivity.UserId].Email
                        : "unknown",
                    Date = dailyActivity.Date,
                    Period = new DateTime(dailyActivity.Date.Year, dailyActivity.Date.Month, 1).ToString("yyyy-MM-dd"),
                    Project = dailyActivity.ProjectId.HasValue
                        ? projects[dailyActivity.ProjectId.Value].Name
                        : string.Empty,
                    Task = dailyActivity.TaskId.HasValue
                        ? tasks[dailyActivity.TaskId.Value].Summary
                        : string.Empty,
                    Tracked = dailyActivity.Tracked,
                    Manual = dailyActivity.Manual,
                    Idle = dailyActivity.Idle,
                    Resumed = dailyActivity.Resumed
                });
            }

            return result;
        }
    
        public IEnumerable<AttendanceSummaryModel> SummarizeAttendance(
            IEnumerable<AttendanceModel> attendanceModels,
            HubStaffActivityResponse? response)
        {
            var attendanceSummaryList = attendanceModels
                .GroupBy(x => new { x.Period, x.Date, x.Email })
                .Select(group => new AttendanceSummaryModel
                {
                    Id = $"{group.Key.Email}_{group.Key.Date:yyyy-MM-dd}",
                    Email = group.Key.Email,
                    Period = group.Key.Period,
                    Date = group.Key.Date,
                    Tracked = group.Sum(x => x.Tracked),
                    Manual = group.Sum(x => x.Manual),
                    Idle = group.Sum(x => x.Idle),
                    Resumed = group.Sum(x => x.Resumed)
                })
                .ToList();

            if (response != null)
            {
                var users = response.Users.ToDictionary(x => x.Id);
                response.Activities = response.Activities.Select(x =>
                {
                    x.StartsAt.AddHours(5);
                    return x;
                });

                attendanceSummaryList.ForEach(summary =>
                {
                    var firstRecord = response.Activities
                        .Where(x => users[x.UserId].Email == summary.Email)
                        .OrderBy(x => x.StartsAt)
                        .FirstOrDefault();

                    if (firstRecord != null)
                    {
                        summary.CheckInTime = firstRecord.StartsAt;
                    }

                    var lastRecord = response.Activities
                        .Where(x => users[x.UserId].Email == summary.Email)
                        .OrderByDescending(x => x.StartsAt)
                        .FirstOrDefault();

                    if (lastRecord != null)
                    {
                        summary.CheckOutTime = lastRecord.StartsAt.AddSeconds(lastRecord.Tracked);
                    }
                });
            }

            return attendanceSummaryList;
        }
    }
}

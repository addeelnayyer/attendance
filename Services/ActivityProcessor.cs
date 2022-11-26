using System;
using System.Collections.Generic;
using System.Linq;
using Aquila360.Attendance.Models;

namespace Aquila360.Attendance.Services
{
	public class ActivityProcessor
	{
		public IEnumerable<AttendanceModel> ProcessDailyActivityResponse(HubStaffDailyActivityResponse response)
		{
			var users = response.Users.ToDictionary(x => x.Id);
            var projects = response.Projects.ToDictionary(x => x.Id);
            var tasks = response.Tasks.ToDictionary(x => x.Id);

			var result = new List<AttendanceModel>();

			foreach(var dailyActivity in response.DailyActivities)
			{
				result.Add(new AttendanceModel
				{
					Id = dailyActivity.Id.ToString(),
					Email = users[dailyActivity.UserId].Email,
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
	}
}

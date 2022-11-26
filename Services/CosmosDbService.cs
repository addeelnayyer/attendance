using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aquila360.Attendance.Models;
using Microsoft.Azure.WebJobs;

namespace Aquila360.Attendance.Services
{
    public class CosmosDbService
    {
        public async Task UpdateAttendance(
            IEnumerable<IAsyncCollector<AttendanceModel>> collectors,
            IEnumerable<AttendanceModel> attendanceModels)
        {
            var tasks = new List<Task>();
            foreach(var collector in collectors)
            {
                foreach (var attendanceModel in attendanceModels)
                {
                    tasks.Add(collector.AddAsync(attendanceModel));
                }
            }

            await Task.WhenAll(tasks);

            tasks.Clear();
            foreach (var collector in collectors)
            {
                tasks.Add(collector.FlushAsync());
            }

            await Task.WhenAll(tasks);
        }
    }
}

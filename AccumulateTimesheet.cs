using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Aquila360.Attendance.Models;
using Aquila360.Attendance.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Aquila360.Attendance
{
    public class AccumulateTimesheet
    {
        [FunctionName("AccumulateTimesheet")]
        public async Task Run(
            [TimerTrigger("0 26 4 * * *")]TimerInfo myTimer, 
            [Table("config", "hubstaff", "access-token", Connection = "HubStaffConnectionString")] ConfigEntity accessTokenConfig,
            [Table("config", "hubstaff", "org-id", Connection = "HubStaffConnectionString")] ConfigEntity orgIdConfig,
            [CosmosDB(
                databaseName: "Attendance",
                collectionName: "AttendanceByEmployee",
                ConnectionStringSetting = "CosmosDBConnection")]
                IAsyncCollector<AttendanceModel> byEmployeeCollector,
            [CosmosDB(
                databaseName: "Attendance",
                collectionName: "AttendanceByPeriod",
                ConnectionStringSetting = "CosmosDBConnection")]
                IAsyncCollector<AttendanceModel> byPeriodCollector,
            [CosmosDB(
                databaseName: "Attendance",
                collectionName: "ActivityLogs",
                ConnectionStringSetting = "CosmosDBConnection")]
                IAsyncCollector<ActivityLogModel> activityLogCollector,
            ILogger log)
        {
            var hubStaffSvc = new HubStaffService(accessTokenConfig.Value, log);

            for (var date = new DateTime(2021, 3, 12); date < DateTime.Today; date = date.AddDays(1))
            {
                var successful = true;
                var message = string.Empty;

                var cosmosDbSvc = new CosmosDbService();
                var response = new HubStaffDailyActivityResponse();

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                try
                {
                    response = await hubStaffSvc.GetActivities(int.Parse(orgIdConfig.Value), date);

                    var activityProcessor = new ActivityProcessor();
                    var attendanceModels = activityProcessor.ProcessDailyActivityResponse(response);

                    var collectors = new List<IAsyncCollector<AttendanceModel>>
                {
                    byEmployeeCollector,
                    byPeriodCollector,
                };

                    await cosmosDbSvc.UpdateAttendance(collectors, attendanceModels);
                }
                catch (Exception ex)
                {
                    successful = false;
                    message = ex.Message;
                }
                finally
                {
                    stopWatch.Stop();
                    var model = new ActivityLogModel
                    {
                        Id = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
                        Date = DateTime.Now.ToString("yyyy-MM-dd"),
                        ActivitiesDate = date,
                        ActivitiesCount = response.DailyActivities.Count(),
                        Duration = stopWatch.ElapsedMilliseconds,
                        Successful = successful,
                        Message = message
                    };

                    await activityLogCollector.AddAsync(model);
                    await activityLogCollector.FlushAsync();
                }
            }
        }
    }
}

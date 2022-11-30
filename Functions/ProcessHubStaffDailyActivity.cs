using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using Aquila360.Attendance.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Aquila360.Attendance.Functions
{
    public class ProcessHubStaffDailyActivity
    {
        private readonly IActivityLogsCosmosService _activityLogsCosmosSvc;
        private readonly IActivityProcessor _activityProcessor;
        private readonly IAttendanceByEmployeeCosmosService _attendanceByEmployeeCosmosSvc;
        private readonly IAttendanceByPeriodCosmosService _attendanceByPeriodCosmosSvc;
        private readonly IAttendanceSummaryByEmployeeCosmosService _attendanceSummaryByEmployeeCosmosSvc;
        private readonly IAttendanceSummaryByPeriodCosmosService _attendanceSummaryByPeriodCosmosSvc;
        private readonly IFirebaseService _firebaseSvc;
        private readonly IHubStaffService _hubStaffSvc;
        private readonly ILogger _log;

        public ProcessHubStaffDailyActivity(
            IActivityLogsCosmosService activityLogsCosmosSvc,
            IActivityProcessor activityProcessor,
            IAttendanceByEmployeeCosmosService attendanceByEmployeeCosmosSvc,
            IAttendanceByPeriodCosmosService attendanceByPeriodCosmosSvc,
            IAttendanceSummaryByEmployeeCosmosService attendanceSummaryByEmployeeCosmosSvc,
            IAttendanceSummaryByPeriodCosmosService attendanceSummaryByPeriodCosmosSvc,
            IFirebaseService firebaseSvc,
            IHubStaffService hubStaffSvc,
            ILoggerFactory loggerFactory)
        {
            _activityLogsCosmosSvc = activityLogsCosmosSvc;
            _activityProcessor = activityProcessor;
            _attendanceByEmployeeCosmosSvc = attendanceByEmployeeCosmosSvc;
            _attendanceByPeriodCosmosSvc = attendanceByPeriodCosmosSvc;
            _attendanceSummaryByEmployeeCosmosSvc = attendanceSummaryByEmployeeCosmosSvc;
            _attendanceSummaryByPeriodCosmosSvc = attendanceSummaryByPeriodCosmosSvc;
            _firebaseSvc = firebaseSvc;
            _hubStaffSvc = hubStaffSvc;
            _log = loggerFactory.CreateLogger<ProcessHubStaffDailyActivity>();
        }

        [Function("ProcessHubStaffDailyActivity")]
        public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer)
        {
            var successful = true;
            var message = string.Empty;

            var response = new HubStaffDailyActivityResponse();

            var date = DateTime.Now;
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                response = await _hubStaffSvc.GetDailyActivities(date);

                if (response != null)
                {
                    var attendanceModels = _activityProcessor.ProcessDailyActivityResponse(response);

                    var activities = await _hubStaffSvc.GetActivities(date);
                    var attendanceSummaryModels = _activityProcessor.SummarizeAttendance(attendanceModels, activities);

                    await _firebaseSvc.UpdateTrackedTime(attendanceSummaryModels);

                    await Task.WhenAll(
                        _attendanceByEmployeeCosmosSvc.BulkUpsert(attendanceModels),
                        _attendanceByPeriodCosmosSvc.BulkUpsert(attendanceModels),
                        _attendanceSummaryByEmployeeCosmosSvc.BulkUpsert(attendanceSummaryModels),
                        _attendanceSummaryByPeriodCosmosSvc.BulkUpsert(attendanceSummaryModels));
                }
                else
                {
                    successful = false;
                    message = $"Failed to process daily activities. Received empty response!";
                }
            }
            catch (Exception ex)
            {
                successful = false;
                message = ex.Message;
            }
            finally
            {
                stopWatch.Stop();
                var model = new ActivityLogModel("DailyActivity", date)
                {
                    ActivitiesDate = date,
                    ActivitiesCount = response?.DailyActivities.Count(),
                    Duration = stopWatch.ElapsedMilliseconds,
                    Successful = successful,
                    Message = message
                };

                await _activityLogsCosmosSvc.Upsert(model);
            }
        }
    }
}

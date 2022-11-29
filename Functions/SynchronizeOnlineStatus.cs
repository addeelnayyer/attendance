using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Aquila360.Attendance.Functions
{
    public class SynchronizeOnlineStatus
    {
        private readonly IActivityLogsCosmosService _activityLogsCosmosSvc;
        private readonly IFirebaseService _firebaseSvc;
        private readonly IHubStaffService _hubStaffSvc;
        private readonly ILogger _logger;

        public SynchronizeOnlineStatus(
            IActivityLogsCosmosService activityLogsCosmosSvc,
            IFirebaseService firebaseSvc,
            IHubStaffService hubStaffSvc,
            ILoggerFactory loggerFactory)
        {
            _activityLogsCosmosSvc = activityLogsCosmosSvc;
            _firebaseSvc = firebaseSvc;
            _hubStaffSvc = hubStaffSvc;

            _logger = loggerFactory.CreateLogger<SynchronizeOnlineStatus>();
        }

        [Function("SynchronizeOnlineStatus")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            var successful = true;
            var message = "Online status sync successful";

            try
            {
                var response = await _hubStaffSvc.GetRecentActivities();

                if (response != null)
                {
                    var syncSuccessful = await _firebaseSvc.UpdateOnlineStatus(response);

                    if (!syncSuccessful)
                    {
                        successful = false;
                        message = $"Failed to update online status. Received errors from firebase!";
                    }
                }
                else
                {
                    successful = false;
                    message = $"Failed to update online status. Received empty response!";
                }
            }
            catch (Exception ex)
            {
                successful = false;
                message = ex.Message;
            }
            finally
            {
                var model = new ActivityLogModel("OnlineStatus", DateTime.Now)
                {
                    Successful = successful,
                    Message = message
                };

                await _activityLogsCosmosSvc.Insert(model);
            }
        }
    }
}

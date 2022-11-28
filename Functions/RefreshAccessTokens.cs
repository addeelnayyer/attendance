using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Aquila360.Attendance.Functions
{
    public class RefreshAccessTokens
    {
        private readonly IActivityLogsCosmosService _activityLogsCosmosSvc;
        private readonly IConfigurationsCosmosService _configCosmosSvc;
        private readonly IConfigurationService _configSvc;
        private readonly IHubStaffService _hubStaffSvc;
        private readonly ILogger _log;

        public RefreshAccessTokens(
            IActivityLogsCosmosService activityLogsCosmosSvc,
            IConfigurationsCosmosService configCosmosSvc,
            IConfigurationService configSvc,
            IHubStaffService hubStaffSvc,
            ILoggerFactory loggerFactory)
        {
            _activityLogsCosmosSvc = activityLogsCosmosSvc;
            _configCosmosSvc = configCosmosSvc;
            _configSvc = configSvc;
            _hubStaffSvc = hubStaffSvc;
            _log = loggerFactory.CreateLogger<ProcessHubStaffDailyActivity>();
        }

        [Function("RefreshAccessTokens")]
        public async Task Run([TimerTrigger("* */30 * * * *")]TimerInfo myTimer)
        {
            try
            {
                var response = await _hubStaffSvc.RefreshAccessToken(await _configSvc.RefreshToken());

                if (response == null || string.IsNullOrWhiteSpace(response.AccessToken)
                    || string.IsNullOrWhiteSpace(response.RefreshToken))
                {
                    var model = new ActivityLogModel("AccessToken", DateTime.Now)
                    {
                        Successful = false,
                        Message = $"Failed to refresh access token. Received empty response!"
                    };

                    await _activityLogsCosmosSvc.Insert(model);

                    return;
                }

                await _configCosmosSvc.BulkUpsert(new List<ConfigurationModel>
                {
                    new ConfigurationModel(Constants.AccessToken, response.AccessToken),
                    new ConfigurationModel(Constants.RefreshToken, response.RefreshToken),
                });
            }
            catch (Exception ex)
            {
                var model = new ActivityLogModel("AccessToken", DateTime.Now)
                {
                    Successful = false,
                    Message = $"Failed to refresh access token. Message: {ex.Message}"
                };

                await _activityLogsCosmosSvc.Insert(model);
            }
        }
    }
}

using System.Net;
using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Aquila360.Attendance.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IFirebaseConfig _fbConfig;
        private readonly ILogger<FirebaseService> _logger;
        private IFirebaseClient? _client;

        public FirebaseService(IConfiguration config, ILoggerFactory loggerFactory)
        {
            _fbConfig = new FirebaseConfig
            {
                BasePath = config.GetValue<string>("Firebase:DatabaseUrl"),
                AuthSecret = config.GetValue<string>("Firebase:DatabaseSecret")
            };

            _logger = loggerFactory.CreateLogger<FirebaseService>();
        }

        private IFirebaseClient Client => _client ?? (_client = new FirebaseClient(_fbConfig));

        public async Task<bool> UpdateOnlineStatus(HubStaffLastActivityResponse lastActivityResponse)
        {
            _logger.LogInformation($"[{nameof(FirebaseService)}] UpdateOnlineStatus");

            if (lastActivityResponse == null || lastActivityResponse.LastActivities == null)
            {
                return false;
            }

            var users = lastActivityResponse.Users.ToDictionary(x => x.Id);

            var tasks = lastActivityResponse.LastActivities.Select(lastActivity =>
            {
                var key = users[lastActivity.UserId].Email.Replace("@aquila360.com", string.Empty).Replace(".", string.Empty);

                return Client.SetAsync(
                    $"onlineStatus/{key}",
                    new
                    {
                        email = users[lastActivity.UserId].Email,
                        name = users[lastActivity.UserId].Name,
                        isOnline = lastActivity.Online,
                        lastActivity = lastActivity.LastClientActivity
                    });
            });

            var setResponses = await Task.WhenAll(tasks);

            return setResponses.All(x => x.StatusCode == HttpStatusCode.OK);
        }

        public async Task<bool> UpdateTrackedTime(IEnumerable<AttendanceSummaryModel> models)
        {
            _logger.LogInformation($"[{nameof(FirebaseService)}] UpdateTrackedTime");

            var trackedTime = models.ToDictionary(x => x.Email.Replace("@aquila360.com", string.Empty).Replace(".", string.Empty));

            var trackedTimeResp = await Client.GetAsync("trackedTime");
            var prevTrackedTime = JsonConvert.DeserializeObject<Dictionary<string, AttendanceSummaryModel>>(trackedTimeResp.Body);

            if (prevTrackedTime != null)
            {
                var deletions = prevTrackedTime
                    .Where(x => !trackedTime.Keys.Contains(x.Key))
                    .Select(x => Client.DeleteAsync($"trackedTime/{x.Key}"));

                await Task.WhenAll(deletions);
            }

            var tasks = trackedTime.Select(x => Client.SetAsync($"trackedTime/{x.Key}", x.Value));

            var setResponses = await Task.WhenAll(tasks);

            return setResponses.All(x => x.StatusCode == HttpStatusCode.OK);
        }
    }
}

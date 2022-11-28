using System.Net;
using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

            var syncSuccessfully = true;

            var users = lastActivityResponse.Users
                .Select(user =>
                {
                    user.Email = user.Email.Replace(".", "_").Replace("@", "_");
                    return user;
                })
                .ToDictionary(x => x.Id);

            lastActivityResponse.LastActivities.ToList().ForEach(lastActivity => {
                if (lastActivity.LastClientActivity.HasValue)
                {
                    lastActivity.LastClientActivity = lastActivity.LastClientActivity.Value.AddHours(5);
                }
            });

            var tasks = lastActivityResponse.LastActivities.Select(lastActivity => 
                Client.SetAsync(
                    $"onlineStatus/{users[lastActivity.UserId].Email}",
                    lastActivity.Online == true));

            _logger.LogInformation($"[{nameof(FirebaseService)}] Waiting for onlineStatus node update.");

            var setResponses = await Task.WhenAll(tasks);

            _logger.LogInformation($"[{nameof(FirebaseService)}] Waiting for onlineStatus node update.");

            syncSuccessfully = syncSuccessfully && setResponses.All(x => x.StatusCode == HttpStatusCode.OK);

            tasks = lastActivityResponse.LastActivities.Select(lastActivity =>
                Client.SetAsync(
                    $"{(lastActivity.Online == true ? "online" : "offline")}/{users[lastActivity.UserId].Email}",
                    lastActivity.LastClientActivity != null ? lastActivity.LastClientActivity : DateTime.MinValue));

            setResponses = await Task.WhenAll(tasks);
            syncSuccessfully = syncSuccessfully && setResponses.All(x => x.StatusCode == HttpStatusCode.OK);

            return syncSuccessfully;
        }
    }
}

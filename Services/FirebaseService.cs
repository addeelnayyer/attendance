using System.Net;
using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Extensions.Configuration;

namespace Aquila360.Attendance.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IFirebaseConfig _fbConfig;
        private IFirebaseClient? _client;

        public FirebaseService(IConfiguration config)
        {
            _fbConfig = new FirebaseConfig
            {
                BasePath = config.GetValue<string>("Firebase:DatabaseUrl"),
                AuthSecret = config.GetValue<string>("Firebase:DatabaseSecret")
            };
        }

        private IFirebaseClient Client => _client ?? (_client = new FirebaseClient(_fbConfig));

        public async Task<bool> UpdateOnlineStatus(HubStaffLastActivityResponse lastActivityResponse)
        {
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

            // var test = lastActivityResponse.LastActivities.Select(lastActivity => $"onlineStatus/{users[lastActivity.UserId].Email}/{lastActivity.Online == true}");

            var tasks = lastActivityResponse.LastActivities.Select(lastActivity => 
                Client.SetAsync(
                    $"onlineStatus/{users[lastActivity.UserId].Email}",
                    lastActivity.Online == true));

            var setResponses = await Task.WhenAll(tasks);
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

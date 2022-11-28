using System;
using System.Net.Http;
using System.Threading.Tasks;
using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Aquila360.Attendance.Services
{
    public class HubStaffService : IHubStaffService
    {
        private readonly IConfigurationService _configSvc;
        private readonly ILogger _log;

        public HubStaffService(IConfigurationService configSvc, ILoggerFactory logFactory)
        {
            _configSvc = configSvc;
            _log = logFactory.CreateLogger<HubStaffService>();
        }

        public async Task<HubStaffDailyActivityResponse?> GetActivities(DateTime date)
        {
            var orgId = await _configSvc.OrgId();
            var urlDate = date.ToString("yyyy-MM-dd");
            var url = $"https://api.hubstaff.com/v2/organizations/{orgId}/activities/daily"
                + $"?date[start]=@{urlDate}T00:00:00Z&date[stop]=@{urlDate}T00:00:00Z"
                + "&include[]=users&include[]=projects&include[]=tasks";

            using var client = new HttpClient();
            await SetupClient(client);
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Status Code: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<HubStaffDailyActivityResponse>(json);
        }

        public async Task<HubStaffLastActivityResponse?> GetRecentActivities()
        {
            var orgId = await _configSvc.OrgId();
            var url = $"https://api.hubstaff.com/v2/organizations/{orgId}/last_activities"
                + "?page_start_id=1&page_limit=500&include[]=users";

            using var client = new HttpClient();
            await SetupClient(client);
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Status Code: {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<HubStaffLastActivityResponse>(json);
        }

        public async Task<HubStaffAccessTokenResponse?> RefreshAccessToken(string refreshToken)
        {
            var url = $"https://account.hubstaff.com/access_tokens?grant_type=refresh_token&refresh_token={refreshToken}";

            using var client = new HttpClient();
            await SetupClient(client);
            var response = await client.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<HubStaffAccessTokenResponse>(json);
            }

            return new HubStaffAccessTokenResponse();
        }

        private async Task SetupClient(HttpClient client)
        {
            var accessToken = await _configSvc.AccessToken();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        }
    }
}

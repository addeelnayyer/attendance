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

        public async Task<HubStaffActivityResponse?> GetActivities(DateTime date)
        {
            var activities = new List<HubStaffActivity>();
            var users = new List<HubStaffUser>();

            var orgId = await _configSvc.OrgId();
            var pageStartAt = 1L;

            while (pageStartAt != 0)
            {
                var startDate = date.ToString("yyyy-MM-dd");
                var endDate = date.AddDays(1).ToString("yyyy-MM-dd");
                var baseUrl = $"https://api.hubstaff.com/v2/organizations/{orgId}/activities";
                var queryString = GetActivitiesQueryString(pageStartAt, startDate, endDate);

                using var client = new HttpClient();
                await SetupClient(client);
                var url = $"{baseUrl}?{queryString}";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Status Code: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var activitiesResponse = JsonConvert.DeserializeObject<HubStaffActivityResponse>(json);

                if (activitiesResponse == null)
                {
                    break;
                }

                activities.AddRange(activitiesResponse.Activities);
                users.AddRange(activitiesResponse.Users.Where(x => users.All(user => user.Id != x.Id)));

                pageStartAt = activitiesResponse.Pagination.NextPageStartAt;
            }

            return new HubStaffActivityResponse
            {
                Activities = activities,
                Users = users
            };
        }

        public async Task<HubStaffDailyActivityResponse?> GetDailyActivities(DateTime date)
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

        private string GetActivitiesQueryString(long startAt, string startDate, string endDate)
        {
            return $"page_start_id={startAt}&page_limit=500&" + 
                $"time_slot[start]={startDate}T00:00:00Z&" + 
                $"time_slot[stop]={endDate}T00:00:00Z&include[]=users";
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

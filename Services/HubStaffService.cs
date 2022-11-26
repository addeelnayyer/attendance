using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Aquila360.Attendance;
using Aquila360.Attendance.Models;
using Azure.Core;
using Microsoft.Extensions.Logging;

namespace Aquila360.Attendance.Services
{
	public class HubStaffService : IDisposable
	{
        private readonly string _accessToken;
        private readonly HttpClient _client;
        private readonly ILogger _log;

        public HubStaffService(string accessToken, ILogger log)
        {
            _accessToken = accessToken;
            _client = new HttpClient();
            _log = log;

            SetupClient();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task<HubStaffDailyActivityResponse> GetActivities(int orgId, DateTime date)
		{
            var urlDate = date.ToString("yyyy-MM-dd");
            var url = $"https://api.hubstaff.com/v2/organizations/{orgId}/activities/daily"
                + $"?date[start]=@{urlDate}T00:00:00Z&date[stop]=@{urlDate}T00:00:00Z"
                + "&include[]=users&include[]=projects&include[]=tasks";

            var response = await _client.GetAsync(url);

            _log.LogError($"HubStaff Daily Activity Response Status Code: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<HubStaffDailyActivityResponse>();
            }

            return new HubStaffDailyActivityResponse();
        }

        private void SetupClient()
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
        }
	}
}

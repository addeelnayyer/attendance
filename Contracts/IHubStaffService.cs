using System;
using System.Threading.Tasks;
using Aquila360.Attendance.Models;

namespace Aquila360.Attendance.Contracts
{
    public interface IHubStaffService
    {
        Task<HubStaffDailyActivityResponse?> GetActivities(DateTime date);

        Task<HubStaffAccessTokenResponse?> RefreshAccessToken(string refreshToken);
    }
}

using Aquila360.Attendance.Models;

namespace Aquila360.Attendance.Contracts
{
    public interface IHubStaffService
    {
        Task<HubStaffActivityResponse?> GetActivities(DateTime date);

        Task<HubStaffDailyActivityResponse?> GetDailyActivities(DateTime date);

        Task<HubStaffLastActivityResponse?> GetRecentActivities();

        Task<HubStaffAccessTokenResponse?> RefreshAccessToken(string refreshToken);
    }
}

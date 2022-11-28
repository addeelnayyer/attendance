using Aquila360.Attendance.Models;

namespace Aquila360.Attendance.Contracts
{
    public interface IFirebaseService
    {
        Task<bool> UpdateOnlineStatus(HubStaffLastActivityResponse lastActivityResponse);
    }
}

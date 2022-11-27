using System.Collections.Generic;
using Aquila360.Attendance.Models;

namespace Aquila360.Attendance.Contracts
{
    public interface IActivityProcessor
    {
        IEnumerable<AttendanceModel> ProcessDailyActivityResponse(HubStaffDailyActivityResponse response);
    }
}

using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using Microsoft.Extensions.Configuration;

namespace Aquila360.Attendance.Services;

public class AttendanceSummaryByEmployeeCosmosService : CosmosService<AttendanceSummaryModel>, IAttendanceSummaryByEmployeeCosmosService
{
    public AttendanceSummaryByEmployeeCosmosService(IConfiguration config) : base(config, "AttendanceSummaryByEmployee")
    {
    }
}

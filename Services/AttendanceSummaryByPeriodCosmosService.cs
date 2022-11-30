using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using Microsoft.Extensions.Configuration;

namespace Aquila360.Attendance.Services;

public class AttendanceSummaryByPeriodCosmosService : CosmosService<AttendanceSummaryModel>, IAttendanceSummaryByPeriodCosmosService
{
    public AttendanceSummaryByPeriodCosmosService(IConfiguration config) : base(config, "AttendanceSummaryByPeriod")
    {
    }
}

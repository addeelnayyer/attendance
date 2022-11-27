using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using Microsoft.Extensions.Configuration;

namespace Aquila360.Attendance.Services;

public class AttendanceByPeriodCosmosService : CosmosService<AttendanceModel>, IAttendanceByPeriodCosmosService
{
    public AttendanceByPeriodCosmosService(IConfiguration config) : base(config, "AttendanceByPeriod")
    {
    }
}

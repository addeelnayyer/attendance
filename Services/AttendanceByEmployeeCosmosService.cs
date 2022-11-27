using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using Microsoft.Extensions.Configuration;

namespace Aquila360.Attendance.Services;

public class AttendanceByEmployeeCosmosService : CosmosService<AttendanceModel>, IAttendanceByEmployeeCosmosService
{
    public AttendanceByEmployeeCosmosService(IConfiguration config) : base(config, "AttendanceByEmployee")
    {
    }
}

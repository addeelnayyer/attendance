using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using Microsoft.Extensions.Configuration;

namespace Aquila360.Attendance.Services;

public class ActivityLogsCosmosService : CosmosService<ActivityLogModel>, IActivityLogsCosmosService
{
    public ActivityLogsCosmosService(IConfiguration config) : base(config, "ActivityLogs")
    {
    }
}

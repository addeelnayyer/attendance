using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;
using Microsoft.Extensions.Configuration;

namespace Aquila360.Attendance.Services;

public class ConfigurationsCosmosService : CosmosService<ConfigurationModel>, IConfigurationsCosmosService
{
    public ConfigurationsCosmosService(IConfiguration config) : base(config, "Configurations")
    {
    }
}

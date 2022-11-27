using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquila360.Attendance.Contracts;
using Aquila360.Attendance.Models;

namespace Aquila360.Attendance.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationsCosmosService _configurationCosmosSvc;
        private IEnumerable<ConfigurationModel>? _configurations;

        public ConfigurationService(IConfigurationsCosmosService configurationCosmosSvc)
        {
            _configurationCosmosSvc = configurationCosmosSvc;
        }

        public async Task<string> AccessToken()
        {
            var configs = await GetConfigurations();
            return configs.First(x => x.Id == Constants.AccessToken).Value;
        }

        public async Task<string> OrgId()
        {
            var configs = await GetConfigurations();
            return configs.First(x => x.Id == Constants.OrgId).Value;
        }

        public async Task<string> RefreshToken()
        {
            var configs = await GetConfigurations();
            return configs.First(x => x.Id == Constants.RefreshToken).Value;
        }

        private async Task<IEnumerable<ConfigurationModel>> GetConfigurations()
        {
            if (_configurations != null)
            {
                return _configurations;
            }

            return _configurations = await _configurationCosmosSvc.RetrieveAllAsync();
        }
    }
}

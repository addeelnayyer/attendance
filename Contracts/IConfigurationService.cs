using System.Collections.Generic;
using System.Threading.Tasks;
using Aquila360.Attendance.Models;

namespace Aquila360.Attendance.Contracts
{
    public interface IConfigurationService
    {
        Task<string> AccessToken();

        Task<string> OrgId();

        Task<string> RefreshToken();
    }
}

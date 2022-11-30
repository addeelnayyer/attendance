using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aquila360.Attendance.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class HubStaffUser
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}

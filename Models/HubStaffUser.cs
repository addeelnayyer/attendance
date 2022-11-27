using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aquila360.Attendance.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class HubStaffUser
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}

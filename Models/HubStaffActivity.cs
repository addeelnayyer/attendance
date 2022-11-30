using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aquila360.Attendance.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class HubStaffActivity
    {
        public long Id { get; set; }

        [JsonProperty("starts_at")]
        public DateTime StartsAt { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("tracked")]
        public int Tracked { get; set; }
    }
}

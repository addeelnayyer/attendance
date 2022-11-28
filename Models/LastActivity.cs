using Newtonsoft.Json;

namespace Aquila360.Attendance.Models
{
    public class LastActivity
    {
        [JsonProperty("last_client_activity")]
        public DateTime? LastClientActivity { get; set; }

        [JsonProperty("online")]
        public bool? Online { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }
    }
}

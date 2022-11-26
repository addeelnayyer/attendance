using Newtonsoft.Json;

namespace Aquila360.Attendance.Models
{
    public class HubStaffUser
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
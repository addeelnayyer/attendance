using Newtonsoft.Json;

namespace Aquila360.Attendance.Models
{
    public class HubStaffTask
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }
    }
}
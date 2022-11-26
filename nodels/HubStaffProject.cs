using Newtonsoft.Json;

namespace Aquila360.Attendance.Models
{
    public class HubStaffProject
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
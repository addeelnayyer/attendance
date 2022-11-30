using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aquila360.Attendance.Models
{
    public class HubStaffPagination
    {
        [JsonProperty("next_page_start_id")]
        public long NextPageStartAt { get; set; } = 0;
    }
}

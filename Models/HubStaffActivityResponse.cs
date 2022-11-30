using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aquila360.Attendance.Models
{
    public class HubStaffActivityResponse
    {
        [JsonProperty("activities")]
        public IEnumerable<HubStaffActivity> Activities { get; set; } = Enumerable.Empty<HubStaffActivity>();

        public IEnumerable<HubStaffUser> Users { get; set; } = Enumerable.Empty<HubStaffUser>();

        public HubStaffPagination Pagination { get; set; } = new HubStaffPagination();
    }
}

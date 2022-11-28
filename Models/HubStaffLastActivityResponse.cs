using Newtonsoft.Json;

namespace Aquila360.Attendance.Models
{
	public class HubStaffLastActivityResponse
	{
		[JsonProperty("last_activities")]
		public IEnumerable<LastActivity> LastActivities { get; set; } = new List<LastActivity>();

		[JsonProperty("users")]
        public IEnumerable<HubStaffUser> Users { get; set; } = new List<HubStaffUser>();
    }
}

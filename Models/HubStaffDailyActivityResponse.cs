using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aquila360.Attendance.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class HubStaffDailyActivityResponse
    {
        [JsonProperty("daily_activities")]
        public IEnumerable<DailyActivity> DailyActivities { get; set; } = Enumerable.Empty<DailyActivity>();

        public IEnumerable<HubStaffUser> Users { get; set; } = Enumerable.Empty<HubStaffUser>();

        public IEnumerable<HubStaffProject> Projects { get; set; } = Enumerable.Empty<HubStaffProject>();

        public IEnumerable<HubStaffTask> Tasks { get; set; } = Enumerable.Empty<HubStaffTask>();
    }
}

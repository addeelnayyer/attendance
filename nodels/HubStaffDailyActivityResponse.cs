using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aquila360.Attendance.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class HubStaffDailyActivityResponse
    {
        [JsonProperty("daily_activities")]
        public IEnumerable<DailyActivity> DailyActivities { get; set; }

        public IEnumerable<HubStaffUser> Users { get; set; }

        public IEnumerable<HubStaffProject> Projects { get; set; }

        public IEnumerable<HubStaffTask> Tasks { get; set; }
    }
}

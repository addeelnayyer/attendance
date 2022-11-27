using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aquila360.Attendance.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DailyActivity
    {
        public long Id { get; set; }

        public DateTime Date { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("project_id")]
        public long? ProjectId { get; set; }

        [JsonProperty("task_id")]
        public long? TaskId { get; set; }

        public int Tracked { get; set; }

        public int Manual { get; set; }

        public int Idle { get; set; }

        public int Resumed { get; set; }
    }
}

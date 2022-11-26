using System;
using Newtonsoft.Json;

namespace Aquila360.Attendance.Models
{
    public class DailyActivity
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("project_id")]
        public long? ProjectId { get; set; }

        [JsonProperty("task_id")]
        public long? TaskId { get; set; }

        [JsonProperty("tracked")]
        public int Tracked { get; set; }

        [JsonProperty("manual")]
        public int Manual { get; set; }

        [JsonProperty("idle")]
        public int Idle { get; set; }

        [JsonProperty("resumed")]
        public int Resumed { get; set; }
    }
}

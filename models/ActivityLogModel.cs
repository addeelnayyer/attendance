using System;

namespace Aquila360.Attendance.Models
{
	public class ActivityLogModel : BaseModel
    {
		public string Date { get; set; }

        public int ActivitiesCount { get; set; }

        public DateTime ActivitiesDate { get; set; }

        public long Duration { get; set; }

        public bool Successful { get; set; }

		public string Message { get; set; }
    }
}

using System;

namespace Aquila360.Attendance.Models
{
	public class ActivityLogModel : BaseModel
    {
        private readonly DateTime _date;

        public ActivityLogModel(DateTime date)
        {
            _date = date;
        }

        public new string Id => _date.ToString("yyyy-MM-ddTHH:mm");

        public string Date => _date.ToString("yyyy-MM-dd");

        public int? ActivitiesCount { get; set; }

        public DateTime ActivitiesDate { get; set; }

        public long Duration { get; set; }

        public bool Successful { get; set; } = false;

		public string Message { get; set; }
    }
}

using System;

namespace Aquila360.Attendance.Models
{
	  public class ActivityLogModel : BaseModel
    {
        private readonly string _category;
        private readonly DateTime _date;

        public ActivityLogModel(string category, DateTime date)
        {
            _category = category;
            _date = date;
        }

        public new string Id => $"{_date:yyyy-MM-ddTHH:mm}";

        public string PartitionKey => $"{_category}_{_date:yyyy-MM-dd}";

        public int? ActivitiesCount { get; set; }

        public DateTime ActivitiesDate { get; set; }

        public long Duration { get; set; }

        public bool Successful { get; set; } = false;

		    public string Message { get; set; } = string.Empty;
    }
}

namespace Aquila360.Attendance.Models
{
    public class AttendanceSummaryModel : BaseModel
	{
        public string Email { get; set; } = string.Empty;

        public string Period { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public DateTime CheckInTime { get; set; }

        public DateTime CheckOutTime { get; set; }

        public int Tracked { get; set; }

        public int Manual { get; set; }

        public int Idle { get; set; }

        public int Resumed { get; set; }

        public int TotalTime => Tracked - Idle;
    }
}

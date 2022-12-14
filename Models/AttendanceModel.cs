namespace Aquila360.Attendance.Models
{
    public class AttendanceModel : BaseModel
	{
        public string Email { get; set; } = string.Empty;

        public string Period { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Project { get; set; } = string.Empty;

        public string Task { get; set; } = string.Empty;

        public int Tracked { get; set; }

        public int Manual { get; set; }

        public int Idle { get; set; }

        public int Resumed { get; set; }
    }
}

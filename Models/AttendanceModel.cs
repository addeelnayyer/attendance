using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aquila360.Attendance.Models
{
    public class AttendanceModel : BaseModel
	{
        public string Email { get; set; }

        public string Period { get; set; }

        public DateTime Date { get; set; }

        public string Project { get; set; }

        public string Task { get; set; }

        public int Tracked { get; set; }

        public int Manual { get; set; }

        public int Idle { get; set; }

        public int Resumed { get; set; }
    }
}

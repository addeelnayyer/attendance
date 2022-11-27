namespace Aquila360.Attendance.Models
{
    public class ExternalUserModel : BaseModel
    {
        public string OrgCode { get; set; }
        
        public string InternalEmail { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}

namespace Aquila360.Attendance.Models
{
    public class ConfigurationModel : BaseModel
    {
        public ConfigurationModel(string id, string value)
        {
            Id = id;
            Value = value;
        }

        public string Value { get; set; }
    }
}

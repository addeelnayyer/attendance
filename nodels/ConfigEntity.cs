using System;
using Azure;
using Azure.Data.Tables;

namespace Aquila360.Attendance.Models
{
    public class ConfigEntity : ITableEntity
    {
        public string Email { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string Value { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }
    }
}

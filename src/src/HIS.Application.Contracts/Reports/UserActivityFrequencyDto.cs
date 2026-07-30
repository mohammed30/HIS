using System;

namespace HIS.Reports
{
    public class UserActivityFrequencyDto
    {
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Module { get; set; }
        public string? EntityType { get; set; }
        public string? Action { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastAccessTime { get; set; }
        public int FrequencyCount { get; set; }
    }
}

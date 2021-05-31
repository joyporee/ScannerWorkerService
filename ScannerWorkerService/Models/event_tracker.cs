using System;

namespace ScannerWorkerService.Models
{
    public class event_tracker
    {
        public int AppId { get; set; }
        public long LastEventId { get; set; }
        public DateTime DateTimeLastUpdated { get; set; }
        public string Description { get; set; }
    }
}

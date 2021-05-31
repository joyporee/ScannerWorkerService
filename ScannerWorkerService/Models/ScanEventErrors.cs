using System.ComponentModel.DataAnnotations.Schema;

namespace ScannerWorkerService.Models
{
    public class ScanEventErrors
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public string Message { get; set; }
        [ForeignKey("EventId")]
        public virtual ScanRecords ScanRecords { get; set; }
    }
}

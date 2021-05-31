using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScannerWorkerService.Models
{
    public class ScanRecords
    {
        public long EventId { get; set; }
        public long ParcelId { get; set; }
        public int ScanTypeId { get; set; }
        
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDateTimeUtc { get; set; }
        public string StatusCode { get; set; }
        public int DeviceId { get; set; }
        public string UserId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? LastUpdateDateTimeUtc { get; set; }

        [ForeignKey("ScanTypeId")]
        public virtual ScanType ScanTypes { get; set; }

        [ForeignKey("DeviceId")]
        public virtual Device Devices { get; set; }

        [ForeignKey("UserId")]
        public virtual Users Users { get; set; }
    }
}

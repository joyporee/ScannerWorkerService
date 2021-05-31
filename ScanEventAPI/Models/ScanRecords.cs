using System;
using System.Collections.Generic;

namespace ScanEventAPI.Models
{

    public class ScanRecords: BaseModel
    {
        public int ScanTypeId { get; set; }
        public int DeviceId { get; set; }
        public string UserId { get; set; }

        public ScanType Type { get; set; }
        public ScanDevice Device { get; set; }
        public ScanUser User { get; set; }

    }

    public class BaseModel 
    {
        public long EventId { get; set; }
        public long ParcelId { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public string StatusCode { get; set; }
    }

    public class ScanType
    {
        public int Id { get; set; }
        public string Description { get; set; }

    }

    public class ScanDevice
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ScanUser
    {
        public string Id { get; set; }
        public string CarrierId { get; set; }
        public int RunId { get; set; }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScanEventAPI.Models;


/// <summary>
/// This is sample ScanEventAPI controller
/// This is constructed to produce sample scan events a random of 100
/// </summary>
namespace ScanEventAPI.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ScansController : ControllerBase
    {

        #region readonlyStaticVariables
        private static readonly string[] Statuses = new[]
        {
            "Delivered", "Ready to collect", "Item on way to collection point", "With courier for delivery", "Processing at depot", "Collected from sender", "Redirection requested", "Delivery attempted - refer to card", "With courier for delivery", "Handed over", "Processing at depot","Tracking number allocated to parcel"
        };
        private static readonly List<ScanType> ScanTypesList =  new List<ScanType>
        {
            new ScanType { Id = 1, Description = "PROCESSING" },
            new ScanType { Id = 2, Description = "PICKUP" },
            new ScanType { Id = 3, Description = "DELIVERY" } 
        };
        private static readonly List<ScanDevice> ScanDeviceList = new List<ScanDevice>
        {
            new ScanDevice { Id = 1, Name = "Device-001001" },
            new ScanDevice { Id = 2, Name = "Device-001002" },
            new ScanDevice { Id = 3, Name = "Device-001003" },
            new ScanDevice { Id = 4, Name = "Device-001004" },
            new ScanDevice { Id = 5, Name = "Device-001005" },
            new ScanDevice { Id = 6, Name = "Device-001006" },
            new ScanDevice { Id = 7, Name = "Device-001007" },
            new ScanDevice { Id = 8, Name = "Device-001008" },
            new ScanDevice { Id = 9, Name = "Device-001009" },
            new ScanDevice { Id = 10, Name = "Device-001010" }
        };
        private static readonly string[] Users = new[]
        {
            "NC1001", "NC1002", "NC1003", "NC1004", "NC1005", "PH1001", "CP1001", "NW1001"
        };
        private static readonly List<ScanUser> UsersList = new List<ScanUser>
        {
            new ScanUser { Id = "CP1001", CarrierId = "CP", RunId = 102 },
            new ScanUser { Id = "NC1001", CarrierId = "NC", RunId = 100 },
            new ScanUser { Id = "NC1002", CarrierId = "NC", RunId = 200 },
            new ScanUser { Id = "NC1003", CarrierId = "NC", RunId = 300 },
            new ScanUser { Id = "NC1004", CarrierId = "NC", RunId = 400 },
            new ScanUser { Id = "NC1005", CarrierId = "NC", RunId = 500 },
            new ScanUser { Id = "NW1001", CarrierId = "NW", RunId = 103 },
            new ScanUser { Id = "PH1001", CarrierId = "PH", RunId = 101 }
        };       
        private readonly ILogger<ScansController> _logger;
        #endregion readonlyStaticVariables

        public ScansController(ILogger<ScansController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get Scan Events
        /// </summary>
        /// <param name="FromEventId"></param>
        /// <param name="Limit"></param>
        /// <returns></returns>
        [HttpGet(Name = "getScanEvents")]
        public async Task<ActionResult<IEnumerable<ScanRecords>>> getScanEvents(int FromEventId = 1, int Limit = 100)
        {
            var rng = new Random();

            //TODO: Need to withdraw the followings
            //To Stop the test until 2,000?
            //As in the real world there will be a limited number of events to scan
            if (FromEventId >= 2000)
            {
                var _scanNoRecords = new List<ScanRecords>();
                return _scanNoRecords.ToArray();
            }

            IEnumerable<ScanRecords> _scanRecordList = Enumerable.Range(FromEventId, Limit).Select(index => new ScanRecords
            {
                EventId = FromEventId++,
                ParcelId = rng.Next(1, 2147483647),
                CreatedDateTimeUtc = DateTime.UtcNow.AddDays(index),
                StatusCode = Statuses[rng.Next(Statuses.Length)],
                ScanTypeId = rng.Next(1, 3),
                DeviceId = rng.Next(1, 10),
                UserId = Users[rng.Next(Users.Length)]
            })
            .ToArray();

            foreach (var item in _scanRecordList)
            {
                item.Type = ScanTypesList[item.ScanTypeId];
                item.Device = ScanDeviceList[item.DeviceId];
                item.User = UsersList.FirstOrDefault(o => o.Id == item.UserId);
            }

            return _scanRecordList.ToArray();
        }
    }
}

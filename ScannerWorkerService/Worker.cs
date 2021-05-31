using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScannerWorkerService.Data;
using ScannerWorkerService.Models;

namespace ScannerWorkerService
{
    public class Worker : BackgroundService
    {
        
        /// <summary>
        /// All variables
        /// </summary>
        private readonly ILogger<Worker> _logger;
        private HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ScannerWSContext _context;
        private IEnumerable<ScanRecords> scanEventRecords { get; set; }


        /// <summary>
        /// Constructor to initiate initial property values 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="serviceProvider"></param>
        public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _context = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ScannerWSContext>();
        }


        /// <summary>
        /// Once the service starting will initiate few resources
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Once the service stops will dispose and stop all the resources
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Dispose();
            _logger.LogInformation("The scanner worker service has been stopped...");
            return base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Every time service executes will loop through this method
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //ScanEventAPI address
                var baseAddress = "https://localhost:44324/v1/scans";

                //reading the last fetched eventId
                long _lastEventId = LastEventId();
                //reading the limit of fetching number of events
                int _limit = Convert.ToInt32(GetConfigurationValue("limit_events_fetching"));
                if (_limit == 0)
                {
                    _limit = 100;    //By default the limit is set to 100 events to fetch
                }
                else
                {
                    //Do nothing; As the vlaue will be supplied by configuration
                    //TODO: Remove the development limit value
                    //_limit = 2;
                }

                var result = await _client.GetAsync(baseAddress + $"?FromEventId={_lastEventId}&Limit={_limit}");
                if (result.IsSuccessStatusCode)
                {
                    _logger.LogInformation("The ScanEventAPI is up and running. Status code {StatusCode}", result.StatusCode);

                    //start scanning events data 
                    //Fetching the data into an object to store and use
                    var customerJsonString = await result.Content.ReadAsStringAsync();
                    IEnumerable<ScanRecords> _scanRecords = JsonConvert.DeserializeObject<IEnumerable<ScanRecords>>(customerJsonString);

                    //No Scan to POST
                    if (_scanRecords.Count() != 0)
                    {
                        PostScannedEvents(_scanRecords);

                        _logger.LogInformation("There are {EventsCount} number(s) of events are fetched and logged into the DB. Last event is {EventId}", _scanRecords.Count(), _scanRecords.Max(o => o.EventId));
                    }
                    else
                    {
                        //May be we don't need to post the following log
                        _logger.LogInformation("There are no events to fetch and log into the DB.");
                    }
                }
                else
                {
                    //Perform some more actions, e.g. email to support or/and logging to database
                    _logger.LogError("The website is down. Status code {StatusCode}", result.StatusCode);
                }

                var delaySec = GetConfigurationValue("delay_task_by_sec");
                if (delaySec == null)
                {
                    await Task.Delay(5 * 1000, stoppingToken);    //By default delay for 5 seconds
                }
                else
                {
                    await Task.Delay(Convert.ToInt32(delaySec) * 1000, stoppingToken);    //supplied the delay time (seconds) from DB
                }
            }
        }

        /// <summary>
        /// Posting all the scanned event records 
        /// </summary>
        /// <param name="scanRecords"></param>
        private void PostScannedEvents(IEnumerable<ScanRecords> scanRecords)
        {
            //If no records found
            if (scanRecords.Count() == 0)
            {
                return;
            }

            //Fetch all the scan Records into DB
            foreach (ScanRecords item in scanRecords)
            {
                ScanRecords itemFromDB = new ScanRecords();
                itemFromDB = EventIdExists(item.EventId);
                //Not exists so add a new
                if (itemFromDB == null)
                {
                    item.CreatedDateTimeUtc = DateTime.UtcNow;
                    _context.ScanRecords.Add(item);
                    _context.SaveChanges();
                }
                //Already exists; so update
                else
                {
                    itemFromDB.ParcelId = item.ParcelId;
                    itemFromDB.StatusCode = item.StatusCode;
                    itemFromDB.ScanTypeId = item.ScanTypeId;
                    itemFromDB.DeviceId = item.DeviceId;
                    itemFromDB.UserId = item.UserId;
                    itemFromDB.LastUpdateDateTimeUtc = DateTime.UtcNow;
                    _context.SaveChanges();
                }
            }

            //Updating the event tracker to refer for the future fetching
            event_tracker _tracker = _context.event_tracker.SingleOrDefault();
            if (_tracker != null)
            {
                _tracker.LastEventId = scanRecords.Max(o => o.EventId);
                _tracker.DateTimeLastUpdated = DateTime.UtcNow;
                _tracker.Description = "{TotalNumberOfEvents} number(s) of events are fetched and logged into the DB. Last event is {LastEventId}; {" + scanRecords.Count().ToString() + "}, {" + _tracker.LastEventId.ToString() + "}.\n";
                _tracker.LastEventId.ToString();
                _context.SaveChanges();
            }

            //return true;
        }


        /// <summary>
        /// Checking if the event already exists; so to chage the status
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private ScanRecords EventIdExists(long id)
        {
            ScanRecords localItem = _context.ScanRecords.FirstOrDefault(o => o.EventId == id);
            return localItem;
        }

        /// <summary>
        /// Getting the Last or Max event ID
        /// </summary>
        /// <returns></returns>
        private long LastEventId()
        {
            var tracker = _context.event_tracker.OrderBy(o=>o.DateTimeLastUpdated).FirstOrDefault();

            if (tracker != null)
            {
                return tracker.LastEventId + 1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Getting the configuration value from Sy_Options
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private string GetConfigurationValue(string config)
        {
            return _configuration[config];
        }

        public override void Dispose()
        {
            _context?.Dispose();
        }
    }
}

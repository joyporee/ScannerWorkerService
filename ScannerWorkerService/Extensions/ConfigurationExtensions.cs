using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ScannerWorkerService.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScannerWorkerService.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddDbProvider(this IConfigurationBuilder configuration, Action<DbContextOptionsBuilder> setup)
        {
            configuration.Add(new DbConfigurationSource(setup));
            return configuration;
        }
    }
}

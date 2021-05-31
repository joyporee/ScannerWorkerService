using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace ScannerWorkerService.Data
{
    public class DbConfigurationProvider : ConfigurationProvider
    {
        private readonly Action<DbContextOptionsBuilder> _options;

        public DbConfigurationProvider(Action<DbContextOptionsBuilder> options)
        {
            _options = options;
        }

        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<ScannerWSContext>();
            _options(builder);

            using (var context = new ScannerWSContext(builder.Options))
            {
                var items = context.ConfigurationValues.AsNoTracking().ToList();

                foreach (var item in items)
                {
                    Data.Add(item.Id, item.Value);
                }
            }
        }
    }
}

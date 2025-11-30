using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceAbstraction.Services.Decorators
{
    public class LoggingDataService : IDataService
    {
        private readonly IDataService _innerService;
        private readonly Action<string> _logger;

        public LoggingDataService(IDataService innerService, Action<string>? logger = null)
        {
            _innerService = innerService ?? throw new ArgumentNullException(nameof(innerService));
            _logger = logger ?? Console.WriteLine;
        }

        public IEnumerable<string> GetLines()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            _logger($"[{timestamp}] DataService.GetLines() called");

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var result = _innerService.GetLines().ToList();
                stopwatch.Stop();

                _logger($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] DataService.GetLines() completed successfully. " +
                       $"Returned {result.Count} lines in {stopwatch.ElapsedMilliseconds}ms");

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] DataService.GetLines() failed after {stopwatch.ElapsedMilliseconds}ms. " +
                       $"Error: {ex.Message}");
                throw;
            }
        }
    }
}

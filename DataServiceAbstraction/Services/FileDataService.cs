using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataServiceAbstraction.Infrastructure;

namespace DataServiceAbstraction.Services
{
    public class FileDataService : IDataService
    {
        private readonly string _filePath;
        private readonly ILogger _logger;

        public FileDataService(string filePath, ILogger logger)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<string> GetLines()
        {
            if (!File.Exists(_filePath))
            {
                _logger.LogError($"File not found: {_filePath}");
                throw new FileNotFoundException($"File not found: {_filePath}");
            }

            _logger.LogInformation($"Reading lines from file: {_filePath}");
            return File.ReadAllLines(_filePath);
        }
    }
}

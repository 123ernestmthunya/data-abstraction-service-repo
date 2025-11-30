using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServiceAbstraction.Services
{

    /// <summary>
    /// Core data service interface following LexisPlus Austria microservice patterns.
    /// Designed for enterprise scalability and observability.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Retrieves data lines with streaming support for large datasets.
        /// </summary>
        /// <returns>Memory-efficient enumerable stream of data lines</returns>
        IEnumerable<string> GetLines();
    }
}

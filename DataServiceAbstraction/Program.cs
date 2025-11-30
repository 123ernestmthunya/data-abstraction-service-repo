using DataServiceAbstraction.Infrastructure;
using DataServiceAbstraction.Services.Decorators;
using DataServiceAbstraction.Services;
using System.Diagnostics;

Console.WriteLine("=== Data Service Abstraction Demo ===");

var logger = new ConsoleLogger();


try
{
    await Run(logger);
}
catch (Exception ex)
{
    Console.WriteLine($"Demo failed: {ex.Message}");
}

Console.WriteLine("\n=== Demo Complete ===");

static async Task Run(ILogger logger)
{
    var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "sample.txt");

    if (!File.Exists(dataPath))
    {
        Console.WriteLine("   Generating sample dataset...");
        CreateDataFile(dataPath);
    }

    IDataService baseService = new FileDataService(dataPath, logger);

    var stopwatch = Stopwatch.StartNew();
    var count = baseService.GetLines().Take(100000).Count();
    stopwatch.Stop();

    Console.WriteLine($"Processed: {count:N0} lines in {stopwatch.ElapsedMilliseconds}ms\n");

    // 2. PERFORMANCE: Cached
    Console.WriteLine("High-Performance Caching (5-minute TTL):");
    IDataService cachedService = new CachedDataService(baseService, TimeSpan.FromMinutes(5), logger);

    // First call - cache miss
    var sw1 = Stopwatch.StartNew();
    var lines1 = cachedService.GetLines().Take(50000).Count();
    sw1.Stop();

    // Second call - cache hit
    var sw2 = Stopwatch.StartNew();
    var lines2 = cachedService.GetLines().Take(50000).Count();
    sw2.Stop();

    Console.WriteLine($"   First call: {lines1:N0} lines in {sw1.ElapsedMilliseconds}ms");
    Console.WriteLine($"   Cached call: {lines2:N0} lines in {sw2.ElapsedMilliseconds}ms (speedup: {sw1.ElapsedMilliseconds / (double)sw2.ElapsedMilliseconds:F1}x)\n");

    IDataService logservice = new LoggingDataService(
        new CachedDataService(
            new FileDataService(dataPath, logger),
            TimeSpan.FromMinutes(10),
            logger),
        message => logger.LogInformation(message));

    var enterpriseCount = logservice.GetLines().Take(75000).Count();
    Console.WriteLine($"   Enterprise pipeline: {enterpriseCount:N0} lines processed\n");
}

static void CreateDataFile(string path)
{
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    File.WriteAllLines(path, Enumerable.Range(1, 1_000_000).Select(i => $"Line {i}"));
}
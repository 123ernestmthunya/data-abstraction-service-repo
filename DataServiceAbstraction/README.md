# Data Service Abstraction Demo

A C# demonstration of the **Decorator Pattern** showcasing how to add functionality (caching, logging) to data services without modifying existing code.

## 📋 Overview

This project demonstrates:
- **Decorator Pattern** implementation
- **SOLID principles** (Open/Closed, Single Responsibility, Dependency Inversion)
- **High-performance caching** with Time-To-Live (TTL)
- **Composition over Inheritance**
- **Separation of Concerns**

## 🛠️ Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher
- Visual Studio 2022, VS Code, or any C# IDE
- Windows, macOS, or Linux

## 🚀 Getting Started

### Clone the Repository

```bash
git clone <repository-url>
cd DataServiceAbstraction
```

### Build the Application

```bash
dotnet build
```

### Run the Application

```bash
dotnet run --project DataServiceAbstraction
```

Or from the project directory:

```bash
cd DataServiceAbstraction
dotnet run
```

## 📂 Project Structure

```
DataServiceAbstraction/
├── DataServiceAbstraction/
│   ├── Services/
│   │   ├── IDataService.cs          # Core abstraction
│   │   ├── FileDataService.cs       # Base implementation
│   │   └── Decorators/
│   │       ├── CachedDataService.cs # Caching decorator
│   │       └── LoggingDataService.cs # Logging decorator
│   ├── Infrastructure/
│   │   ├── ILogger.cs
│   │   └── ConsoleLogger.cs
│   ├── Program.cs                   # Demo application
│   └── data/                        # Auto-generated data files
└── README.md
```

## 🎯 What the Demo Does

### Demo 1: Baseline Performance
Reads 100,000 lines from a file without any decorators.

```
Processed: 100,000 lines in ~120ms
```

### Demo 2: Caching Decorator
Shows performance improvement with caching:
- **First call**: Cache MISS - reads from file
- **Second call**: Cache HIT - serves from memory

```
First call: 50,000 lines in ~150ms
Cached call: 50,000 lines in ~0ms (speedup: ∞x)
```

### Demo 3: Decorator Composition
Combines multiple decorators (Logging + Caching):

```csharp
IDataService enterprise = new LoggingDataService(
    new CachedDataService(
        new FileDataService()));
```

## 🔧 Configuration

### Cache TTL
Modify cache expiration time in `Program.cs`:

```csharp
// 5-minute cache
new CachedDataService(baseService, TimeSpan.FromMinutes(5), logger);

// 10-minute cache
new CachedDataService(baseService, TimeSpan.FromMinutes(10), logger);
```

### Data File Size
Change the number of generated lines in `CreateDataFile()`:

```csharp
File.WriteAllLines(path, Enumerable.Range(1, 1_000_000).Select(i => $"Line {i}"));
//                                         ^^^^^^^^^ Change this number
```

## 🧪 Running Tests

```bash
dotnet test
```

## 📊 Expected Output

```
=== Data Service Abstraction Demo ===
   Processed: 100,000 lines in 112ms

🔸 High-Performance Caching (5-minute TTL):
   First call: 50,000 lines in 141ms
   Cached call: 50,000 lines in 0ms (speedup: ∞x)

[2025-11-30 12:09:58] DataService.GetLines() called
   Enterprise pipeline: 75,000 lines processed

=== Demo Complete ===
```

## 🎓 Learning Points

### 1. **Decorator Pattern**
Add functionality without modifying existing classes:
```csharp
IDataService service = new CachedDataService(new FileDataService());
```

### 2. **SOLID Principles**
- **Open/Closed**: Open for extension, closed for modification
- **Single Responsibility**: Each class has one job
- **Dependency Inversion**: Depend on abstractions (`IDataService`)

### 3. **Composition over Inheritance**
Stack decorators instead of deep inheritance hierarchies.

## 🐛 Troubleshooting

### "File not found" Error
The application automatically generates `data/sample.txt` on first run. If you see this error:
```bash
# Manually create the data directory
mkdir -p DataServiceAbstraction/data
```

### Build Errors
Ensure you have .NET 8.0 SDK installed:
```bash
dotnet --version
```
# DotNetDemo – .NET Core 3.1 Web API

.NET Core 3.1 Web API with demo endpoints, a **Products** API (in-memory DB), unit tests, and JMeter performance tests.

## API endpoints

**Demo**
| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/demo/ping` | Returns `"pong"` (health check) |
| GET | `/api/demo/echo?text=...` | Returns `{ "echoed": "..." }` |
| POST | `/api/demo/sum` | Body: `{ "A": 1, "B": 2 }` → `{ "Result": 3 }` |

**Products (in-memory database)**
| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/products` | Returns all products (seeded with 3 items on startup) |
| POST | `/api/products` | Body: `{ "name": "...", "price": 0 }` → creates product, returns 201 with product |

## Build and run

```bash
dotnet build
dotnet run --project DotNetDemoApi
```

API runs at **http://localhost:5000**.

## Unit tests

```bash
dotnet test
```

Includes **security tests** (see `DotNetDemoApi.Tests/SecurityTests.cs`):
- Security headers on all responses (X-Content-Type-Options, X-Frame-Options, Referrer-Policy, X-XSS-Protection)
- Input validation: name length, negative price, invalid JSON, missing body
- Safe handling of XSS/SQL-injection-style payloads (no crash; content returned as data)

## JMeter performance tests

1. Start the API: `dotnet run --project DotNetDemoApi`
2. Install [Apache JMeter](https://jmeter.apache.org/download_jmeter.cgi).
3. Run the plan (from repo root):

   **GUI:**  
   `jmeter -t jmeter/DotNetDemoApi-Performance.jmx`

   **CLI (recommended for load):**  
   `jmeter -n -t jmeter/DotNetDemoApi-Performance.jmx -l jmeter/results.jtl -e -o jmeter/report`

See **jmeter/README.md** for details.

## Code quality and static analysis

The solution uses:

- **.editorconfig** – Code style (indentation, naming, braces, etc.) applied in the IDE and at build.
- **.NET analyzers** (`Microsoft.CodeAnalysis.NetAnalyzers`) – CA rules for security, performance, maintainability, and design. Configured in **Directory.Build.props** with `AnalysisLevel = latest-recommended` and `EnforceCodeStyleInBuild = true`.
- **Directory.Build.props** – Shared settings for all projects (analyzers, analysis level). Set `TreatWarningsAsErrors` to `true` to fail the build on analyzer warnings.

**Commands:**

```bash
# Build (runs analyzers; style and CA warnings appear here)
dotnet build

# Apply .editorconfig style (format and fix style issues)
dotnet format
```

To enforce zero analyzer warnings in CI, set in `Directory.Build.props`:  
`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`.

## Solution layout

- **DotNetDemoApi** – Web API project (includes security headers middleware)  
- **DotNetDemoApi.Tests** – xUnit unit tests + security tests  
- **jmeter/** – JMeter test plan and README  
- **.editorconfig** – Code style and analyzer overrides  
- **Directory.Build.props** – Shared code analysis settings  

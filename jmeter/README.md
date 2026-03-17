# JMeter Performance Tests for DotNetDemoApi

## Prerequisites

- [Apache JMeter](https://jmeter.apache.org/download_jmeter.cgi) (5.x recommended)
- DotNetDemoApi running at `http://localhost:5000`

## Running the API

From the solution root:

```bash
dotnet run --project DotNetDemoApi
```

## Running the performance test

1. **GUI mode** (edit or run with UI):
   ```bash
   jmeter -t DotNetDemoApi-Performance.jmx
   ```

2. **Command-line (non-GUI)** for actual load and results:
   ```bash
   jmeter -n -t DotNetDemoApi-Performance.jmx -l results.jtl -e -o report
   ```
   - `-n` = non-GUI
   - `-t` = test plan
   - `-l` = results file (e.g. `results.jtl`)
   - `-e` = generate report after run
   - `-o` = report output folder (e.g. `report`)

## Test plan summary

- **Thread Group**: 5 users, 2s ramp-up, 10 loops (250 requests per run).
- **Endpoints**:
  - `GET /api/demo/ping` – health check
  - `GET /api/demo/echo?text=hello` – echo
  - `POST /api/demo/sum` – body `{"A":10,"B":20}`
  - `GET /api/products` – list all products (in-memory DB)
  - `POST /api/products` – body `{"name":"Load Test Product","price":19.99}`

Default host/port are set in **HTTP Request Defaults** (localhost:5000). Change them there if your API runs elsewhere.

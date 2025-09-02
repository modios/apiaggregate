```markdown
# 🌐 Aggregated Insights API

An ASP.NET Core Web API that aggregates data from three external sources — weather, tech news,
and global country metadata — into a single unified response. Designed for fast, parallel data fetching,
clean JSON output, and enhanced control via sorting, filtering, authentication, and rate limiting.

---

## 🚀 Features

- 🌤 **Weather Data** from Open-Meteo
- 📰 **Tech News Item** from Hacker News API
- 🌍 **Country Metadata** from World Bank API
- 🔐 **JWT Authentication** for secure access
- 📊 **Request Performance Monitoring** via custom middleware
- ⏱️ **Rate Limiting** per IP using ASP.NET Core's built-in limiter
- 📈 **Logging** with Serilog (console + rolling file logs)

---

## 📦 Aggregated Endpoint

### `GET /api/Aggregation`

Retrieves weather info, a Hacker News item, and country metadata in one call.

#### 🔧 Query Parameters

| Name               | Type     | Required | Description                                                                 |
|--------------------|----------|----------|-----------------------------------------------------------------------------|
| `city`             | string   | ✅       | City name for weather lookup                                               |
| `hackerNewsItemId` | int      | ✅       | Hacker News item ID                                                        |
| `countryCode`      | string   | ✅       | ISO 2-letter country code (e.g. `GR`)                                      |
| `sortBy`           | enum     | ❌       | Optional. Sort response by `Date`                                          |
| `sortOrder`        | enum     | ❌       | Optional. Sort direction: `Asc` or `Desc`                                  |
| `category`         | enum     | ❌       | Optional. Filter response by category: `Weather`, `HackerNews`, `WorldBank` |

---

### 📘 Example Request

```http
GET https://localhost:7145/api/Aggregation?City=Athens&CountryCode=GR&HackerNewsItemId=1312&Category=HackerNews
```

---

### ✅ Example Response

```json
{
  "items": [
    {
      "source": "HackerNews",
      "category": "HackerNews",
      "date": "2007-02-27T01:48:57Z",
      "rawData": {
        "by": "Alex3917",
        "id": 1312,
        "parent": 1245,
        "text": "...",
        "time": 1172548137,
        "type": "comment",
        "createdAt": "2007-02-27T01:48:57Z",
        "category": "HackerNews"
      }
    }
  ]
}
```

---

## 🔐 Authentication

This API uses **JWT Bearer Tokens** for authentication.

### 🔑 Login Endpoint

```http
POST /api/Auth/login
```

#### Request Body

```json
{
  "username": "your-username",
  "password": "your-password"
}
```

> ⚠️ Note: Password validation is currently simulated. Replace with real user authentication logic in production.

#### Response

```json
{
  "token": "your-jwt-token"
}
```

Use this token in the `Authorization` header for authenticated requests:

```
Authorization: Bearer your-jwt-token
```

---

## ⏱️ Rate Limiting

- **Policy**: Fixed window
- **Limit**: 10 requests per minute per IP
- **Queue**: Up to 5 requests
- Applied via `[EnableRateLimiting]` on controllers

---

## 📊 Request Stats

### `GET /api/Stats`

Returns average execution time per endpoint.

#### Example Response

```json
{
  "/api/Aggregation": {
    "averageDurationMs": 123.45,
    "requestCount": 10
  }
}
```

---

## 🧱 Architecture Overview

- `OpenMeteoService` → Fetches weather data using static city coordinates
- `HackerNewsService` → Retrieves a single Hacker News item by ID
- `WorldBankCountryService` → Fetches country metadata by ISO code
- `AggregationService` → Combines all three into a single response, applies sort/filter logic
- `RequestStatsMiddleware` → Measures and stores request durations
- `PerformanceAnalyzerService` → Background service for performance monitoring
- `JwtTokenService` → Generates JWT tokens
- `IRequestStatsStore` → In-memory store for request metrics

---

## 🛠️ Running the Project in Visual Studio

1. Open the solution in **Visual Studio**.
2. Build and run the project.
3. Navigate to the Swagger UI to explore and test the endpoints:

```url
https://localhost:7145/swagger/index.html
```

> 💡 The Swagger interface provides a convenient way to interact with the `/api/Aggregation` endpoint and view live responses.

---

## 📁 Folder Structure (Simplified)

```
ApiAggregator/
├── API/
│   ├── Controllers/
│   ├── Middleware/
│   └── Constants/
├── Core/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Models/
│   └── Enums/
├── Infrastructure/
│   ├── Services/
│   └── Monitoring/
```

---

## 🧪 Testing

Unit tests are written using **xUnit** and **Moq**. To run tests:

```bash
dotnet test
```

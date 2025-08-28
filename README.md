```markdown
# 🌐 Aggregated Insights API

This ASP.NET Core Web API aggregates data from three external sources — weather, tech news, and global country metadata — into a single unified response. It’s designed for fast, parallel data fetching and clean JSON output.

---

## 🚀 Features

- 🌤 **Weather Data** from [Open-Meteo](https://open-meteo.com/)
- 📰 **Tech News Item** from [Hacker News API](https://github.com/HackerNews/API)
- 🌍 **Country Metadata** from [World Bank API](https://datahelpdesk.worldbank.org/knowledgebase/articles/898590-api-country-queries)

---

## 📦 Aggregated Endpoint

### `GET /api/Aggregation`

Retrieves weather info, a Hacker News item, and country metadata in one call.

#### 🔧 Query Parameters

| Name              | Type   | Required | Description                              |
|-------------------|--------|----------|------------------------------------------|
| `city`            | string | ✅       | City name for weather lookup             |
| `hackerNewsItemId`| int    | ✅       | Hacker News item ID                      |
| `countryCode`     | string | ✅       | ISO 2-letter country code (e.g. `GR`)    |

---

### 📘 Example Request

```http
GET https://localhost:7145/api/Aggregation?city=Athens&hackerNewsItemId=1213&countryCode=GR
```

---

### ✅ Example Response

```json
{
  "weather": {
    "city": "Athens",
    "description": "Clear sky",
    "temperature": 28.7,
    "windSpeed": 12.3,
    "humidity": 42
  },
  "hackerNewsItem": {
    "id": 1213,
    "by": "pg",
    "title": "Lisp as the Language of the Future",
    "text": "Lisp has been around for decades...",
    "type": "story",
    "score": 57,
    "time": 1175714200
  },
  "worldBankCountry": {
    "id": "GRC",
    "iso2Code": "GR",
    "name": "Greece",
    "region": "Europe & Central Asia",
    "adminRegion": "",
    "incomeLevel": "High income",
    "lendingType": "IBRD",
    "capitalCity": "Athens",
    "latitude": 37.9838,
    "longitude": 23.7275
  }
}
```

---

## 🧱 Architecture Overview

- `OpenMeteoService` → Fetches weather data using static city coordinates
- `HackerNewsService` → Retrieves a single Hacker News item by ID
- `WorldBankCountryService` → Fetches country metadata by ISO code
- `AggregationService` → Combines all three into a single response
- `AggregationController` → Exposes the `/api/Aggregation` endpoint

---
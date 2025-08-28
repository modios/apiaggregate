```markdown
# 🌐 Aggregated Insights API

This ASP.NET Core Web API aggregates data from three external sources — weather, tech news, and global country metadata — into a single unified response. It’s designed for fast, parallel data fetching, clean JSON output, and now includes sorting and filtering options for enhanced control.

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

| Name               | Type   | Required | Description                                                                 |
|--------------------|--------|----------|-----------------------------------------------------------------------------|
| `city`             | string | ✅       | City name for weather lookup                                               |
| `hackerNewsItemId` | int    | ✅       | Hacker News item ID                                                        |
| `countryCode`      | string | ✅       | ISO 2-letter country code (e.g. `GR`)                                      |
| `sortBy`           | string | ❌       | Optional. Sort response by `date`                                          |
| `sortOrder`        | string | ❌       | Optional. Sort direction: asc or desc                                      |
| `filterBy`         | string | ❌       | Optional. Filter response by Category : Environment, Comment, Economy      |

---

### 📘 Example Request

```http
GET  https://localhost:7145/api/Aggregation?City=Athens&CountryCode=GR&HackerNewsItemId=1312&Category=Comment
```

---

### ✅ Example Response

```json
{
  "items": [
    {
      "source": "HackerNews",
      "category": "Comment",
      "date": "2007-02-27T01:48:57Z",
      "rawData": {
        "by": "Alex3917",
        "id": 1312,
        "parent": 1245,
        "text": "In theory couldn&#39;t one ban any OpenID below a certain pagerank? For example, my OpenID is embedded on my homepage, which has a pagerank of 6. So then could I create a Reddit clone and ban anyone with an OpenID coming from a site with a pagerank of below 4? You would probably have to accept only OpenID&#39;s from the header of index.html, and check to make sure there was only one OpenID per page. That way if you got banned for trolling then you&#39;d have to make a new homepage and get it up to a certain pagerank before you could make a new account at the site.",
        "time": 1172548137,
        "type": "comment",
        "createdAt": "2007-02-27T01:48:57Z",
        "category": "Comment"
      }
    }
  ]
}
```

---

## 🧱 Architecture Overview

- `OpenMeteoService` → Fetches weather data using static city coordinates
- `HackerNewsService` → Retrieves a single Hacker News item by ID
- `WorldBankCountryService` → Fetches country metadata by ISO code
- `AggregationService` → Combines all three into a single response, applies sort/filter logic
- `AggregationController` → Exposes the `/api/Aggregation` endpoint

---

## 🛠️ Running the Project in Visual Studio

To run the API locally:

1. Open the solution in **Visual Studio**.
2. Build and run the project.
3. Navigate to the Swagger UI to explore and test the endpoints:

```url
https://localhost:7145/swagger/index.html
```

> 💡 The Swagger interface provides a convenient way to interact with the `/api/Aggregation` endpoint and view live responses.

---

```

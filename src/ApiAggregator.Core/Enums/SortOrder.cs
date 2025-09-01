using System.Text.Json.Serialization;

namespace ApiAggregator.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortOrder
{
    Asc,
    Desc
}
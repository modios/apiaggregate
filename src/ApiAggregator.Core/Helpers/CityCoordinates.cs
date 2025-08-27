namespace ApiAggregator.Core.Helpers
{
    public static class CityCoordinates
    {
        public static readonly Dictionary<string, (double Latitude, double Longitude)> Coordinates = new()
    {
        { "Athens", (37.98, 23.73) },
        { "Thessaloniki", (40.64, 22.94) },
        { "Patras", (38.25, 21.73) },
        { "Heraklion", (35.34, 25.13) },
        { "Larissa", (39.64, 22.41) },
        { "Volos", (39.37, 22.95) },
        { "Ioannina", (39.67, 20.85) },
        { "Kavala", (40.94, 24.41) },
        { "Rhodes", (36.44, 28.22) },
        { "Chania", (35.51, 24.02) }
    };

        public static (double Latitude, double Longitude)? GetCoordinates(string city)
        {
            if (Coordinates.TryGetValue(city, out var coords))
                return coords;

            return null; // City not found
        }
    }
}

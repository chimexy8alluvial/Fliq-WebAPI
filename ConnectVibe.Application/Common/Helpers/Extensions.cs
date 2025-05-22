using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Common.Helpers
{
    public static class Extensions
    {
        public static int CalculateAge(this DateTime dateOfBirth)
        {
            // Get today's date
            DateTime today = DateTime.Today;

            // Calculate the difference in years
            int age = today.Year - dateOfBirth.Year;

            // Check if the birthday has occurred this year
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula implementation
            double R = 6371; // Earth radius in km

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // Distance in km
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static double CalculateEventSimilarity(Events event1, Events event2)
        {
            double similarity = 0;

            // Basic property comparison
            if (event1.EventType == event2.EventType) similarity += 0.3;
            if (event1.EventCategory == event2.EventCategory) similarity += 0.2;

            // Location similarity
            if (event1.Location != null && event2.Location != null)
            {
                double distance = CalculateDistance(
                    event1.Location.Lat, event1.Location.Lng,
                    event2.Location.Lat, event2.Location.Lng);

                similarity += Math.Max(0, 5 - distance) / 10;
            }

            return similarity;
        }
    }
}

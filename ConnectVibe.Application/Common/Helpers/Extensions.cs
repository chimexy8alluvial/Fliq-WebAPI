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

    }
}

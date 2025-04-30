namespace Fliq.Application.DashBoard.Common
{
    public class WeeklyCountResult
    {
        public Dictionary<DayOfWeek, int> DailyCounts { get; set; }

        public WeeklyCountResult(Dictionary<DayOfWeek, int> dailyCounts)
        {
            DailyCounts = dailyCounts;
        }
    }
}
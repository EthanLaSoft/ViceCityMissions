namespace ViceCityMissions.Models;

public class DashboardViewModel
{
    public List<Mission> Missions { get; set; } = new();
    public HashSet<int> CompletedToday { get; set; } = new();
    public int TotalPoints { get; set; }
    public int PointsToday { get; set; }
    public string Rank { get; set; } = "";
    public int NextRankAt { get; set; }
}
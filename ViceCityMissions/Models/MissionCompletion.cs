namespace ViceCityMissions.Models;

public class MissionCompletion
{
    public int Id { get; set; }
    public int MissionId { get; set; }
    public Mission? Mission { get; set; }
    public DateOnly CompletedOn { get; set; }
    public int PointsEarned { get; set; }
}
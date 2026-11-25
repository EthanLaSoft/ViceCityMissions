using System.ComponentModel.DataAnnotations;

namespace ViceCityMissions.Models;

public class Mission
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string Icon { get; set; } = "🎯";

    [Range(1, 1000, ErrorMessage = "Los puntos deben estar entre 1 y 1000")]
    public int Points { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<MissionCompletion> Completions { get; set; } = new();
}
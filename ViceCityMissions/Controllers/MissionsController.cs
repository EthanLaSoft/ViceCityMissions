using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViceCityMissions.Data;
using ViceCityMissions.Models;

namespace ViceCityMissions.Controllers;

public class MissionsController : Controller
{
    private readonly AppDbContext _db;
    public MissionsController(AppDbContext db) => _db = db;

    private static DateOnly Today => DateOnly.FromDateTime(DateTime.Now);

    // GET: /Missions  -> lista de misiones + puntuación total
    public async Task<IActionResult> Index()
    {
        var today = Today;

        var vm = new DashboardViewModel
        {
            Missions = await _db.Missions.Where(m => m.IsActive).OrderBy(m => m.Id).ToListAsync(),
            CompletedToday = (await _db.Completions
                .Where(c => c.CompletedOn == today)
                .Select(c => c.MissionId).ToListAsync()).ToHashSet(),
            TotalPoints = await _db.Completions.SumAsync(c => (int?)c.PointsEarned) ?? 0,
            PointsToday = await _db.Completions
                .Where(c => c.CompletedOn == today)
                .SumAsync(c => (int?)c.PointsEarned) ?? 0
        };

        (vm.Rank, vm.NextRankAt) = GetRank(vm.TotalPoints);
        return View(vm);
    }

    // GET: /Missions/Create
    public IActionResult Create() => View(new Mission());

    // POST: /Missions/Create  -> crear misión con puntos
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,Icon,Points")] Mission mission)
    {
        if (!ModelState.IsValid) return View(mission);

        if (string.IsNullOrWhiteSpace(mission.Icon)) mission.Icon = "🎯";
        _db.Missions.Add(mission);
        await _db.SaveChangesAsync();

        TempData["Msg"] = $"Misión «{mission.Title}» creada.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Missions/SetPoints/5  -> asignar/cambiar puntos
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPoints(int id, int points)
    {
        var mission = await _db.Missions.FindAsync(id);
        if (mission is null) return NotFound();

        if (points < 1 || points > 1000)
        {
            TempData["Msg"] = "Los puntos deben estar entre 1 y 1000.";
            return RedirectToAction(nameof(Index));
        }

        mission.Points = points;
        await _db.SaveChangesAsync();
        TempData["Msg"] = $"«{mission.Title}» ahora vale {points} puntos.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Missions/Complete/5  -> completar misión (1 vez al día)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var mission = await _db.Missions.FindAsync(id);
        if (mission is null || !mission.IsActive) return NotFound();

        var today = Today;
        var already = await _db.Completions.AnyAsync(c => c.MissionId == id && c.CompletedOn == today);
        if (already)
        {
            TempData["Msg"] = "Esa misión ya fue completada hoy.";
            return RedirectToAction(nameof(Index));
        }

        _db.Completions.Add(new MissionCompletion
        {
            MissionId = id,
            CompletedOn = today,
            PointsEarned = mission.Points
        });
        await _db.SaveChangesAsync();

        TempData["Msg"] = $"¡Misión cumplida! +{mission.Points} puntos.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Missions/Undo/5  -> deshacer la completada de hoy
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Undo(int id)
    {
        var today = Today;
        var c = await _db.Completions.FirstOrDefaultAsync(x => x.MissionId == id && x.CompletedOn == today);
        if (c is not null)
        {
            _db.Completions.Remove(c);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: /Missions/Delete/5  -> desactivar (conserva historial)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var mission = await _db.Missions.FindAsync(id);
        if (mission is not null)
        {
            mission.IsActive = false;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private static (string rank, int nextAt) GetRank(int pts) => pts switch
    {
        < 200 => ("Novato de Vice City", 200),
        < 600 => ("Hustler de Ocean Drive", 600),
        < 1500 => ("Capo de Leonida", 1500),
        _ => ("Kingpin", 0)
    };
}
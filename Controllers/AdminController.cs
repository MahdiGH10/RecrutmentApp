using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecruitApp.Models;
using RecruitApp.Services;
using RecruitApp.ViewModels;

namespace RecruitApp.Controllers;

[Authorize(Roles = "Admin")]
/// <summary>
/// Administration area for managing users, recruiters and global KPIs.
/// </summary>
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOffreService _offreService;
    private readonly ICandidatureService _candidatureService;

    public AdminController(
        UserManager<ApplicationUser> userManager,
        IOffreService offreService,
        ICandidatureService candidatureService)
    {
        _userManager = userManager;
        _offreService = offreService;
        _candidatureService = candidatureService;
    }

    /// <summary>
    /// GET: /Admin/Dashboard
    /// Shows administrative dashboard with KPIs.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var recruteurs = await _userManager.GetUsersInRoleAsync("Recruteur");
        var candidats = await _userManager.GetUsersInRoleAsync("Candidat");
        var candidatures = await _candidatureService.GetAllAsync();

        var candidatureStats = candidatures
            .GroupBy(c => c.Statut)
            .ToDictionary(g => g.Key, g => g.Count());

        candidatureStats.TryGetValue(RecruitApp.Models.Enums.StatutCandidature.EnAttente, out var enAttente);
        candidatureStats.TryGetValue(RecruitApp.Models.Enums.StatutCandidature.Vue, out var vues);
        candidatureStats.TryGetValue(RecruitApp.Models.Enums.StatutCandidature.Acceptee, out var acceptees);
        candidatureStats.TryGetValue(RecruitApp.Models.Enums.StatutCandidature.Refusee, out var refusees);
        candidatureStats.TryGetValue(RecruitApp.Models.Enums.StatutCandidature.EntretienPlanifie, out var entretiensPlanifies);

        var totalOffresPubliees = await _offreService.CountAllAsync();
        var totalOffresActives = await _offreService.CountActiveAsync();

        var model = new AdminDashboardViewModel
        {
            TotalOffres = totalOffresActives,
            TotalOffresPubliees = totalOffresPubliees,
            TotalOffresInactives = Math.Max(totalOffresPubliees - totalOffresActives, 0),
            TotalCandidatures = await _candidatureService.CountAllAsync(),
            TotalEntretiens = await _candidatureService.CountEntretiensAsync(),
            TotalRecruteurs = recruteurs.Count,
            RecruteursValides = recruteurs.Count(r => r.IsValidated),
            RecruteursEnAttente = recruteurs.Count(r => !r.IsValidated),
            TotalCandidats = candidats.Count,
            CandidaturesEnAttente = enAttente,
            CandidaturesVues = vues,
            CandidaturesAcceptees = acceptees,
            CandidaturesRefusees = refusees,
            CandidaturesEntretiensPlanifies = entretiensPlanifies,
            Recruteurs = recruteurs,
            Candidats = candidats
        };

        return View(model);
    }

    /// <summary>
    /// GET: /Admin/Recruteurs
    /// Lists recruiter accounts for validation and management.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Recruteurs()
    {
        var recruteurs = await _userManager.GetUsersInRoleAsync("Recruteur");
        return View(recruteurs);
    }

    /// <summary>
    /// GET: /Admin/RecruteurDetails/{id}
    /// Shows details for a recruiter account.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> RecruteurDetails(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null || !await _userManager.IsInRoleAsync(user, "Recruteur"))
        {
            return NotFound();
        }

        return View(user);
    }

    /// <summary>
    /// POST: /Admin/Valider
    /// Validates a recruiter account.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Valider(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is not null)
        {
            user.IsValidated = true;
            await _userManager.UpdateAsync(user);
        }

        return RedirectToAction(nameof(Recruteurs));
    }
}

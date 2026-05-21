using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecruitApp.Models;
using RecruitApp.Services;
using RecruitApp.ViewModels;

namespace RecruitApp.Controllers;

[Authorize(Roles = "Admin")]
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

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var recruteurs = await _userManager.GetUsersInRoleAsync("Recruteur");
        var candidats = await _userManager.GetUsersInRoleAsync("Candidat");

        var model = new AdminDashboardViewModel
        {
            TotalOffres = await _offreService.CountActiveAsync(),
            TotalCandidatures = await _candidatureService.CountAllAsync(),
            TotalEntretiens = await _candidatureService.CountEntretiensAsync(),
            TotalRecruteurs = recruteurs.Count,
            TotalCandidats = candidats.Count,
            Recruteurs = recruteurs,
            Candidats = candidats
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Recruteurs()
    {
        var recruteurs = await _userManager.GetUsersInRoleAsync("Recruteur");
        return View(recruteurs);
    }

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

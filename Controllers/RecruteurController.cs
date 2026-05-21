using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecruitApp.Models;
using RecruitApp.Models.Enums;
using RecruitApp.Services;
using RecruitApp.ViewModels;

namespace RecruitApp.Controllers;

[Authorize(Roles = "Recruteur")]
public class RecruteurController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOffreService _offreService;
    private readonly ICandidatureService _candidatureService;

    public RecruteurController(
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
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        ViewBag.TotalOffres = await _offreService.CountByRecruiterAsync(user.Id);
        ViewBag.TotalCandidatures = await _candidatureService.CountForRecruiterAsync(user.Id);
        ViewBag.TotalActives = await _offreService.CountActiveAsync();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> MesOffres()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var offres = await _offreService.GetByRecruiterAsync(user.Id);
        return View(new OffreListViewModel { Offres = offres });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new OffreFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OffreFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        await _offreService.AddAsync(new Offre
        {
            Titre = model.Titre,
            Description = model.Description,
            SalaireMin = model.SalaireMin,
            SalaireMax = model.SalaireMax,
            ExperienceRequise = model.ExperienceRequise,
            Zone = model.Zone,
            Type = model.Type,
            IsActive = model.IsActive,
            ExpiresAt = model.ExpiresAt,
            RecruteurId = user.Id,
            PublieeAt = DateTime.UtcNow
        });

        TempData["SuccessMessage"] = "Offre créée avec succès.";
        return RedirectToAction(nameof(MesOffres));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var offre = await _offreService.GetByIdAndRecruiterAsync(id, user.Id);
        if (offre is null)
        {
            return NotFound();
        }

        return View(new OffreFormViewModel
        {
            Id = offre.Id,
            Titre = offre.Titre,
            Description = offre.Description,
            SalaireMin = offre.SalaireMin,
            SalaireMax = offre.SalaireMax,
            ExperienceRequise = offre.ExperienceRequise,
            Zone = offre.Zone,
            Type = offre.Type,
            IsActive = offre.IsActive,
            ExpiresAt = offre.ExpiresAt
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(OffreFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var offre = await _offreService.GetByIdAndRecruiterAsync(model.Id, user.Id);
        if (offre is null)
        {
            return NotFound();
        }

        offre.Titre = model.Titre;
        offre.Description = model.Description;
        offre.SalaireMin = model.SalaireMin;
        offre.SalaireMax = model.SalaireMax;
        offre.ExperienceRequise = model.ExperienceRequise;
        offre.Zone = model.Zone;
        offre.Type = model.Type;
        offre.IsActive = model.IsActive;
        offre.ExpiresAt = model.ExpiresAt;

        await _offreService.UpdateAsync(offre);

        TempData["SuccessMessage"] = "Offre mise à jour.";
        return RedirectToAction(nameof(MesOffres));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var offre = await _offreService.GetByIdAndRecruiterAsync(id, user.Id);
        if (offre is null)
        {
            return NotFound();
        }

        await _offreService.DeleteAsync(offre);
        TempData["SuccessMessage"] = "Offre supprimée.";
        return RedirectToAction(nameof(MesOffres));
    }

    [HttpGet]
    public async Task<IActionResult> Candidatures()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var candidatures = await _candidatureService.GetByRecruiterAsync(user.Id);
        return View(new CandidatureViewModel { Candidatures = candidatures });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangerStatut(int id, StatutCandidature statut)
    {
        await _candidatureService.UpdateStatusAsync(id, statut);
        TempData["SuccessMessage"] = "Statut mis à jour.";
        return RedirectToAction(nameof(Candidatures));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlanifierEntretien(int candidatureId, DateTime dateHeure, string lieu, string? notes)
    {
        await _candidatureService.ScheduleInterviewAsync(candidatureId, dateHeure, lieu, notes);
        TempData["SuccessMessage"] = "Entretien planifié.";
        return RedirectToAction(nameof(Candidatures));
    }
}

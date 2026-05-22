using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecruitApp.Models;
using RecruitApp.Models.Enums;
using RecruitApp.Services;
using RecruitApp.ViewModels;

namespace RecruitApp.Controllers;

[Authorize(Roles = "Recruteur")]
/// <summary>
/// Controller for recruiter actions: manage offers and review candidatures.
/// </summary>
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

    /// <summary>
    /// GET: /Recruteur/Dashboard
    /// Recruiter dashboard overview.
    /// </summary>
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

    /// <summary>
    /// GET: /Recruteur/MesOffres
    /// Lists the authenticated recruiter's offers.
    /// </summary>
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

    /// <summary>
    /// GET: /Recruteur/Create
    /// Show form to create a new offer.
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View(new OffreFormViewModel());
    }

    /// <summary>
    /// POST: /Recruteur/Create
    /// Persist a new offer for the authenticated recruiter.
    /// </summary>
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

    /// <summary>
    /// GET: /Recruteur/Edit/{id}
    /// Show form to edit an existing offer (owner only).
    /// </summary>
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

    /// <summary>
    /// POST: /Recruteur/Edit
    /// Save changes to an offer (owner only).
    /// </summary>
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

    /// <summary>
    /// POST: /Recruteur/Delete
    /// Delete an offer owned by the recruiter.
    /// </summary>
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

    /// <summary>
    /// GET: /Recruteur/Candidatures
    /// Lists candidatures for the authenticated recruiter with filters.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Candidatures(string? search, StatutCandidature? statut, int? offreId, bool onlyInterviews = false)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var candidatures = await _candidatureService.GetByRecruiterAsync(user.Id);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            candidatures = candidatures.Where(c =>
                (c.Candidat?.Prenom?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.Candidat?.Nom?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.Candidat?.Email?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.Offre?.Titre?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
        }

        if (statut.HasValue)
        {
            candidatures = candidatures.Where(c => c.Statut == statut.Value).ToList();
        }

        if (offreId.HasValue)
        {
            candidatures = candidatures.Where(c => c.OffreId == offreId.Value).ToList();
        }

        if (onlyInterviews)
        {
            candidatures = candidatures.Where(c => c.Entretien is not null).ToList();
        }

        candidatures = candidatures
            .Where(c => c.Statut != StatutCandidature.Refusee && string.IsNullOrWhiteSpace(c.Entretien?.DeclineReason))
            .ToList();

        return View(new CandidatureViewModel
        {
            Candidatures = candidatures,
            Search = search,
            Statut = statut,
            OfferFilterId = offreId,
            OnlyInterviews = onlyInterviews
        });
    }

    /// <summary>
    /// POST: /Recruteur/ChangerStatut
    /// Change the status of a candidature.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangerStatut(int id, StatutCandidature statut, string? search = null, StatutCandidature? currentStatut = null, int? offreId = null, bool onlyInterviews = false)
    {
        await _candidatureService.UpdateStatusAsync(id, statut);
        TempData["SuccessMessage"] = "Statut mis à jour.";
        return RedirectToAction(nameof(Candidatures), new { search, statut = currentStatut, offreId, onlyInterviews });
    }

    /// <summary>
    /// POST: /Recruteur/PlanifierEntretien
    /// Schedule an interview for a candidature.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlanifierEntretien(int candidatureId, DateTime dateHeure, string lieu, string? notes, string? search = null, StatutCandidature? currentStatut = null, int? offreId = null, bool onlyInterviews = false)
    {
        await _candidatureService.ScheduleInterviewAsync(candidatureId, dateHeure, lieu, notes);
        TempData["SuccessMessage"] = "Entretien planifié.";
        return RedirectToAction(nameof(Candidatures), new { search, statut = currentStatut, offreId, onlyInterviews });
    }
}

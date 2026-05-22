using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitApp.Models;
using RecruitApp.Services;
using RecruitApp.ViewModels;

namespace RecruitApp.Controllers;

/// <summary>
/// Controller responsible for public and candidate-facing offers pages.
/// </summary>
public class OffresController : Controller
{
    private readonly IOffreService _offreService;
    private readonly ICandidatureService _candidatureService;
    private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

    public OffresController(
        IOffreService offreService,
        ICandidatureService candidatureService,
        Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
    {
        _offreService = offreService;
        _candidatureService = candidatureService;
        _userManager = userManager;
    }

    /// <summary>
    /// GET: /Offres
    /// Lists public offers with optional filters (search, zone, type).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? zone, RecruitApp.Models.Enums.TypeOffre? type)
    {
        var offres = await _offreService.GetPublicOffersAsync(search, zone, type);
        return View(new OffreListViewModel
        {
            Offres = offres,
            Search = search,
            Zone = zone,
            Type = type
        });
    }

    /// <summary>
    /// GET: /Offres/Detail/{id}
    /// Shows details for a single offer. If the user is a candidate, includes application state.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var offre = await _offreService.GetByIdAsync(id);
        if (offre is null)
        {
            return NotFound();
        }

        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
        var isCandidate = User.IsInRole("Candidat");
        var currentUser = isAuthenticated ? await _userManager.GetUserAsync(User) : null;
        var application = currentUser is not null && isCandidate
            ? await _candidatureService.GetCandidateApplicationAsync(id, currentUser.Id)
            : null;

        return View(new OffreDetailViewModel
        {
            Offre = offre,
            CanApply = isCandidate,
            IsAuthenticated = isAuthenticated,
            IsCandidate = isCandidate,
            HasApplied = application is not null,
            ApplicationStatus = application?.Statut,
            ApplicationDate = application?.PostuleeAt,
            InterviewDate = application?.Entretien?.DateHeure,
            InterviewConfirmed = application?.Entretien?.IsConfirmedByCandidat,
            HintMessage = !isAuthenticated
                ? "Connectez-vous pour postuler."
                : isCandidate
                    ? null
                    : "Le bouton de candidature est réservé aux comptes candidats."
        });
    }

    /// <summary>
    /// POST: /Offres/Postuler/{id}
    /// Candidate action to submit an application to an offer.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Candidat")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Postuler(int id, CandidatureSubmitViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var offre = await _offreService.GetByIdAsync(id);
        if (offre is null || !offre.IsActive)
        {
            return NotFound();
        }

        var alreadyApplied = await _candidatureService.HasAppliedAsync(id, user.Id);
        if (alreadyApplied)
        {
            TempData["InfoMessage"] = "Vous avez déjà postulé à cette offre.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        await _candidatureService.AddAsync(new Candidature
        {
            OffreId = id,
            CandidatId = user.Id,
            MessageMotivation = model.MessageMotivation,
            CvUrl = model.CvUrl,
            PostuleeAt = DateTime.UtcNow,
            Statut = RecruitApp.Models.Enums.StatutCandidature.EnAttente
        });

        TempData["SuccessMessage"] = "Votre candidature a été envoyée.";
        return RedirectToAction(nameof(Detail), new { id });
    }
}

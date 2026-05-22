using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecruitApp.Models;
using RecruitApp.Services;
using RecruitApp.ViewModels;

namespace RecruitApp.Controllers;

[Authorize(Roles = "Candidat")]
/// <summary>
/// Controller for candidate area: candidatures, entretiens and document management.
/// </summary>
public class CandidatController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICandidatureService _candidatureService;
    private readonly IWebHostEnvironment _environment;

    public CandidatController(
        UserManager<ApplicationUser> userManager,
        ICandidatureService candidatureService,
        IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _candidatureService = candidatureService;
        _environment = environment;
    }

    /// <summary>
    /// GET: /Candidat/Candidatures
    /// Shows the current user's candidatures.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Candidatures()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var candidatures = await _candidatureService.GetByCandidateAsync(user.Id);
        await _candidatureService.MarkCandidateNotificationsSeenAsync(user.Id);
        return View(new CandidatureViewModel { Candidatures = candidatures });
    }

    /// <summary>
    /// GET: /Candidat/Entretiens
    /// Lists interviews for the authenticated candidate.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Entretiens()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var entretiens = await _candidatureService.GetEntretiensByCandidateAsync(user.Id);
        return View(entretiens);
    }

    /// <summary>
    /// GET: /Candidat/Documents
    /// Shows uploaded documents for the candidate.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Documents()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var documents = await _candidatureService.GetDocumentsByUserAsync(user.Id);
        ViewBag.UploadModel = new DocumentUploadViewModel();
        return View(documents);
    }

    /// <summary>
    /// POST: /Candidat/UploadDocument
    /// Upload a new document for the authenticated candidate.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadDocument(DocumentUploadViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid || model.File is null || model.File.Length == 0)
        {
            TempData["ErrorMessage"] = "Veuillez sélectionner un fichier valide.";
            return RedirectToAction(nameof(Documents));
        }

        var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "documents", user.Id);
        Directory.CreateDirectory(uploadsRoot);

        var extension = Path.GetExtension(model.File.FileName);
        var safeFileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsRoot, safeFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await model.File.CopyToAsync(stream);
        }

        var publicUrl = $"/uploads/documents/{user.Id}/{safeFileName}";

        await _candidatureService.AddDocumentAsync(new Document
        {
            Nom = model.Nom,
            TypeDocument = model.TypeDocument,
            Url = publicUrl,
            UploadedAt = DateTime.UtcNow,
            UserId = user.Id
        });

        TempData["SuccessMessage"] = "Document téléversé avec succès.";
        return RedirectToAction(nameof(Documents));
    }

    /// <summary>
    /// POST: /Candidat/DeleteDocument
    /// Deletes a previously uploaded document (owner only).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var document = await _candidatureService.GetDocumentByIdAsync(id, user.Id);
        if (document is null)
        {
            return NotFound();
        }

        var relativePath = document.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(_environment.WebRootPath, relativePath);
        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }

        await _candidatureService.DeleteDocumentAsync(document);

        TempData["SuccessMessage"] = "Document supprimé.";
        return RedirectToAction(nameof(Documents));
    }

    /// <summary>
    /// POST: /Candidat/ConfirmerEntretien
    /// Candidate confirms or refuses an interview (refuse requires a reason).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmerEntretien(int entretienId, bool confirmed = true, string? declineReason = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (!confirmed && string.IsNullOrWhiteSpace(declineReason))
        {
            TempData["ErrorMessage"] = "Merci d'ajouter un motif si vous refusez l'entretien.";
            return RedirectToAction(nameof(Entretiens));
        }

        await _candidatureService.ConfirmInterviewAsync(entretienId, user.Id, confirmed, declineReason);
        TempData["SuccessMessage"] = confirmed ? "Entretien confirmé." : "Refus enregistré avec votre motif.";
        return RedirectToAction(nameof(Entretiens));
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RecruitApp.Models;

/// <summary>
/// Extension d'IdentityUser pour stocker des informations supplémentaires sur l'utilisateur.
/// Important: <see cref="IsValidated"/> contrôle si un recruteur peut se connecter avant validation admin.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string Prenom { get; set; } = string.Empty;

    public string Nom { get; set; } = string.Empty;

    public string? PhotoUrl { get; set; }

    public string? CompanyName { get; set; }

    public string? CompanyDomain { get; set; }

    public string? CompanyWebsite { get; set; }

    public string? CompanyAddress { get; set; }

    public string? CompanyDescription { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsValidated { get; set; } = false;

    /// <summary>
    /// Candidatures soumises par l'utilisateur (si candidat).
    /// </summary>
    public ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();

    /// <summary>
    /// Offres publiées par l'utilisateur (si recruteur).
    /// </summary>
    public ICollection<Offre> Offres { get; set; } = new List<Offre>();

    /// <summary>
    /// Documents uploadés (CVs, etc.).
    /// </summary>
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}

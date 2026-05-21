using Microsoft.AspNetCore.Identity;

namespace RecruitApp.Models;

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

    public ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();

    public ICollection<Offre> Offres { get; set; } = new List<Offre>();

    public ICollection<Document> Documents { get; set; } = new List<Document>();
}

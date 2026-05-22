using RecruitApp.Models.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RecruitApp.Models;


public class Offre
{
    public int Id { get; set; }

    public string Titre { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal SalaireMin { get; set; }

    public decimal SalaireMax { get; set; }

    public int ExperienceRequise { get; set; }

    public string Zone { get; set; } = string.Empty;

    public TypeOffre Type { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime PublieeAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public string RecruteurId { get; set; } = string.Empty;

 
    public ApplicationUser? Recruteur { get; set; }


    public ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();
}

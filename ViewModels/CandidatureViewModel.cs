using RecruitApp.Models;
using RecruitApp.Models.Enums;

namespace RecruitApp.ViewModels;

public class CandidatureViewModel
{
    public string? Search { get; set; }

    public StatutCandidature? Statut { get; set; }

    public int? OfferFilterId { get; set; }

    public bool OnlyInterviews { get; set; }

    public int OffreId { get; set; }

    public string? MessageMotivation { get; set; }

    public string? CvUrl { get; set; }

    public IEnumerable<Candidature> Candidatures { get; set; } = new List<Candidature>();

    public IEnumerable<Entretien> Entretiens { get; set; } = new List<Entretien>();

    public IEnumerable<Document> Documents { get; set; } = new List<Document>();
}

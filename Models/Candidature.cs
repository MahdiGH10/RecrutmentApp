using RecruitApp.Models.Enums;

namespace RecruitApp.Models;

public class Candidature
{
    public int Id { get; set; }

    public DateTime PostuleeAt { get; set; }

    public StatutCandidature Statut { get; set; } = StatutCandidature.EnAttente;

    public bool IsSeenByCandidat { get; set; } = true;

    public string? MessageMotivation { get; set; }

    public string? CvUrl { get; set; }

    public string CandidatId { get; set; } = string.Empty;

    public ApplicationUser? Candidat { get; set; }

    public int OffreId { get; set; }

    public Offre? Offre { get; set; }

    public Entretien? Entretien { get; set; }
}

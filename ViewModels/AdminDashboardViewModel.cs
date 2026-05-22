using RecruitApp.Models;

namespace RecruitApp.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalOffres { get; set; }

    public int TotalOffresPubliees { get; set; }

    public int TotalOffresInactives { get; set; }

    public int TotalCandidatures { get; set; }

    public int TotalEntretiens { get; set; }

    public int TotalRecruteurs { get; set; }

    public int RecruteursValides { get; set; }

    public int RecruteursEnAttente { get; set; }

    public int TotalCandidats { get; set; }

    public int CandidaturesEnAttente { get; set; }

    public int CandidaturesVues { get; set; }

    public int CandidaturesAcceptees { get; set; }

    public int CandidaturesRefusees { get; set; }

    public int CandidaturesEntretiensPlanifies { get; set; }

    public IEnumerable<ApplicationUser> Recruteurs { get; set; } = new List<ApplicationUser>();

    public IEnumerable<ApplicationUser> Candidats { get; set; } = new List<ApplicationUser>();
}

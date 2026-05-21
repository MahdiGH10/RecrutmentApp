using RecruitApp.Models;

namespace RecruitApp.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalOffres { get; set; }

    public int TotalCandidatures { get; set; }

    public int TotalEntretiens { get; set; }

    public int TotalRecruteurs { get; set; }

    public int TotalCandidats { get; set; }

    public IEnumerable<ApplicationUser> Recruteurs { get; set; } = new List<ApplicationUser>();

    public IEnumerable<ApplicationUser> Candidats { get; set; } = new List<ApplicationUser>();
}

using RecruitApp.Models;
using RecruitApp.Models.Enums;

namespace RecruitApp.ViewModels;

public class OffreListViewModel
{
    public IEnumerable<Offre> Offres { get; set; } = new List<Offre>();

    public string? Search { get; set; }

    public string? Zone { get; set; }

    public TypeOffre? Type { get; set; }
}

using RecruitApp.Models;
using RecruitApp.Models.Enums;

namespace RecruitApp.ViewModels;

public class OffreDetailViewModel
{
    public Offre? Offre { get; set; }

    public bool CanApply { get; set; }

    public bool IsAuthenticated { get; set; }

    public bool IsCandidate { get; set; }

    public bool HasApplied { get; set; }

    public StatutCandidature? ApplicationStatus { get; set; }

    public DateTime? ApplicationDate { get; set; }

    public DateTime? InterviewDate { get; set; }

    public bool? InterviewConfirmed { get; set; }

    public string? HintMessage { get; set; }
}

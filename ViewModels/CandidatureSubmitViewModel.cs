using System.ComponentModel.DataAnnotations;

namespace RecruitApp.ViewModels;

public class CandidatureSubmitViewModel
{
    public int OffreId { get; set; }

    [StringLength(4000)]
    public string? MessageMotivation { get; set; }

    [StringLength(1000)]
    public string? CvUrl { get; set; }
}
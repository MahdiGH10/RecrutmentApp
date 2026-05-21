using System.ComponentModel.DataAnnotations;
using RecruitApp.Models.Enums;

namespace RecruitApp.ViewModels;

public class OffreFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Titre { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal SalaireMin { get; set; }

    [Range(0, double.MaxValue)]
    public decimal SalaireMax { get; set; }

    [Range(0, 50)]
    public int ExperienceRequise { get; set; }

    [Required]
    [StringLength(120)]
    public string Zone { get; set; } = string.Empty;

    [Required]
    public TypeOffre Type { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? ExpiresAt { get; set; }
}
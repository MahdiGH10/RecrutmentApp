using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RecruitApp.ViewModels;

public class DocumentUploadViewModel
{
    [Required]
    [StringLength(120)]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string TypeDocument { get; set; } = string.Empty;

    [Required]
    public IFormFile? File { get; set; }
}

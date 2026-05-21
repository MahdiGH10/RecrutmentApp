using System.ComponentModel.DataAnnotations;

namespace RecruitApp.ViewModels;

public class RegisterViewModel : IValidatableObject
{
    [Required]
    public AccountType AccountType { get; set; } = AccountType.Candidat;

    [Required]
    [StringLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(150)]
    public string? CompanyName { get; set; }

    [StringLength(100)]
    public string? CompanyDomain { get; set; }

    [Url]
    [StringLength(200)]
    public string? CompanyWebsite { get; set; }

    [StringLength(200)]
    public string? CompanyAddress { get; set; }

    [StringLength(800)]
    public string? CompanyDescription { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Les mots de passe ne correspondent pas.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (AccountType != AccountType.Recruteur)
        {
            yield break;
        }

        if (string.IsNullOrWhiteSpace(CompanyName))
        {
            yield return new ValidationResult(
                "Le nom de l'entreprise est obligatoire pour un compte recruteur.",
                new[] { nameof(CompanyName) });
        }

        if (string.IsNullOrWhiteSpace(CompanyDomain))
        {
            yield return new ValidationResult(
                "Le domaine d'activité est obligatoire pour un compte recruteur.",
                new[] { nameof(CompanyDomain) });
        }
    }
}

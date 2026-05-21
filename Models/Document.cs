namespace RecruitApp.Models;

public class Document
{
    public int Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string TypeDocument { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }
}

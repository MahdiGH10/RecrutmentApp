using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RecruitApp.Models;


public class Entretien
{
    public int Id { get; set; }

    public DateTime DateHeure { get; set; }

    public string Lieu { get; set; } = string.Empty;

    public bool IsConfirmedByCandidat { get; set; } = false;

    public string? DeclineReason { get; set; }

    public string? Notes { get; set; }

    public int CandidatureId { get; set; }

 
    public Candidature? Candidature { get; set; }
}

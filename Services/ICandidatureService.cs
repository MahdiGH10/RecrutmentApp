using RecruitApp.Models;
using RecruitApp.Models.Enums;

namespace RecruitApp.Services;

// Candidature service
public interface ICandidatureService
{
    Task<List<Candidature>> GetByCandidateAsync(string candidateId);

    Task<List<Candidature>> GetByOfferAsync(int offreId);

    Task<List<Candidature>> GetByRecruiterAsync(string recruiterId);

    Task<Candidature?> GetByIdAsync(int id);

    Task<List<Entretien>> GetEntretiensByCandidateAsync(string candidateId);

    Task<List<Document>> GetDocumentsByUserAsync(string userId);

    Task<Document?> GetDocumentByIdAsync(int id, string userId);

    Task<bool> HasAppliedAsync(int offreId, string candidateId);
    
    Task<Candidature?> GetCandidateApplicationAsync(int offreId, string candidateId);

    Task<int> CountForRecruiterAsync(string recruiterId);

    Task<int> CountUnreadCandidateNotificationsAsync(string candidateId);

    Task<int> CountAllAsync();

    Task<int> CountEntretiensAsync();

    Task<List<Candidature>> GetAllAsync();

    Task AddAsync(Candidature candidature);

    Task AddDocumentAsync(Document document);

    Task DeleteDocumentAsync(Document document);

    Task UpdateStatusAsync(int candidatureId, StatutCandidature statut);

    Task ScheduleInterviewAsync(int candidatureId, DateTime dateHeure, string lieu, string? notes);

    Task ConfirmInterviewAsync(int entretienId, string candidateId, bool confirmed, string? declineReason = null);

    Task MarkCandidateNotificationsSeenAsync(string candidateId);
}

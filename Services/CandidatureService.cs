using Microsoft.EntityFrameworkCore;
using RecruitApp.Data;
using RecruitApp.Models;
using RecruitApp.Models.Enums;

namespace RecruitApp.Services;

public class CandidatureService : ICandidatureService
{
    private readonly AppDbContext _context;

    public CandidatureService(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Candidature>> GetByCandidateAsync(string candidateId)
    {
        return _context.Candidatures
            .AsNoTracking()
            .Include(c => c.Offre)
            .Include(c => c.Entretien)
            .Where(c => c.CandidatId == candidateId)
            .OrderByDescending(c => c.PostuleeAt)
            .ToListAsync();
    }

    public Task<List<Candidature>> GetByOfferAsync(int offreId)
    {
        return _context.Candidatures
            .AsNoTracking()
            .Include(c => c.Offre)
            .Include(c => c.Candidat)
            .Include(c => c.Entretien)
            .Where(c => c.OffreId == offreId)
            .OrderByDescending(c => c.PostuleeAt)
            .ToListAsync();
    }

    public Task<List<Candidature>> GetByRecruiterAsync(string recruiterId)
    {
        return _context.Candidatures
            .AsNoTracking()
            .Include(c => c.Offre)
            .Include(c => c.Candidat)
            .Include(c => c.Entretien)
            .Where(c => c.Offre!.RecruteurId == recruiterId)
            .OrderByDescending(c => c.PostuleeAt)
            .ToListAsync();
    }

    public Task<Candidature?> GetByIdAsync(int id)
    {
        return _context.Candidatures
            .Include(c => c.Offre)
            .Include(c => c.Candidat)
            .Include(c => c.Entretien)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public Task<List<Entretien>> GetEntretiensByCandidateAsync(string candidateId)
    {
        return _context.Entretiens
            .AsNoTracking()
            .Include(e => e.Candidature!)
            .ThenInclude(c => c.Offre)
            .Where(e => e.Candidature != null && e.Candidature.CandidatId == candidateId)
            .OrderByDescending(e => e.DateHeure)
            .ToListAsync();
    }

    public Task<List<Document>> GetDocumentsByUserAsync(string userId)
    {
        return _context.Documents
            .AsNoTracking()
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();
    }

    public Task<Document?> GetDocumentByIdAsync(int id, string userId)
    {
        return _context.Documents.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
    }

    public Task<bool> HasAppliedAsync(int offreId, string candidateId)
    {
        return _context.Candidatures.AnyAsync(c => c.OffreId == offreId && c.CandidatId == candidateId);
    }

    public Task<Candidature?> GetCandidateApplicationAsync(int offreId, string candidateId)
    {
        return _context.Candidatures
            .AsNoTracking()
            .Include(c => c.Entretien)
            .FirstOrDefaultAsync(c => c.OffreId == offreId && c.CandidatId == candidateId);
    }

    public Task<int> CountForRecruiterAsync(string recruiterId)
    {
        return _context.Candidatures.CountAsync(c => c.Offre!.RecruteurId == recruiterId);
    }

    public Task<int> CountUnreadCandidateNotificationsAsync(string candidateId)
    {
        return _context.Candidatures.CountAsync(c => c.CandidatId == candidateId && !c.IsSeenByCandidat);
    }

    public Task<int> CountAllAsync()
    {
        return _context.Candidatures.CountAsync();
    }

    public Task<int> CountEntretiensAsync()
    {
        return _context.Entretiens.CountAsync();
    }

    public async Task AddAsync(Candidature candidature)
    {
        _context.Candidatures.Add(candidature);
        await _context.SaveChangesAsync();
    }

    public async Task AddDocumentAsync(Document document)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDocumentAsync(Document document)
    {
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int candidatureId, StatutCandidature statut)
    {
        var candidature = await _context.Candidatures.FirstOrDefaultAsync(c => c.Id == candidatureId);
        if (candidature is null)
        {
            return;
        }

        candidature.Statut = statut;
        candidature.IsSeenByCandidat = false;
        await _context.SaveChangesAsync();
    }

    public async Task ScheduleInterviewAsync(int candidatureId, DateTime dateHeure, string lieu, string? notes)
    {
        var candidature = await _context.Candidatures
            .Include(c => c.Entretien)
            .FirstOrDefaultAsync(c => c.Id == candidatureId);

        if (candidature is null)
        {
            return;
        }

        if (candidature.Entretien is null)
        {
            candidature.Entretien = new Entretien
            {
                DateHeure = dateHeure,
                Lieu = lieu,
                Notes = notes,
                CandidatureId = candidature.Id,
                IsConfirmedByCandidat = false
            };
            _context.Entretiens.Add(candidature.Entretien);
        }
        else
        {
            candidature.Entretien.DateHeure = dateHeure;
            candidature.Entretien.Lieu = lieu;
            candidature.Entretien.Notes = notes;
            candidature.Entretien.IsConfirmedByCandidat = false;
        }

        candidature.Statut = StatutCandidature.EntretienPlanifie;
        candidature.IsSeenByCandidat = false;
        await _context.SaveChangesAsync();
    }

    public async Task ConfirmInterviewAsync(int entretienId, string candidateId, bool confirmed)
    {
        var entretien = await _context.Entretiens
            .Include(e => e.Candidature)
            .FirstOrDefaultAsync(e => e.Id == entretienId && e.Candidature!.CandidatId == candidateId);

        if (entretien is null)
        {
            return;
        }

        entretien.IsConfirmedByCandidat = confirmed;
        await _context.SaveChangesAsync();
    }

    public async Task MarkCandidateNotificationsSeenAsync(string candidateId)
    {
        var candidatures = await _context.Candidatures
            .Where(c => c.CandidatId == candidateId && !c.IsSeenByCandidat)
            .ToListAsync();

        if (candidatures.Count == 0)
        {
            return;
        }

        foreach (var candidature in candidatures)
        {
            candidature.IsSeenByCandidat = true;
        }

        await _context.SaveChangesAsync();
    }
}

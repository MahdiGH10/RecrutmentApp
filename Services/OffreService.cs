using Microsoft.EntityFrameworkCore;
using RecruitApp.Data;
using RecruitApp.Models;
using RecruitApp.Models.Enums;

namespace RecruitApp.Services;

public class OffreService : IOffreService
{
    private readonly AppDbContext _context;

    public OffreService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Offre>> GetPublicOffersAsync(string? search = null, string? zone = null, TypeOffre? type = null)
    {
        var query = _context.Offres
            .AsNoTracking()
            .Include(o => o.Recruteur)
            .Where(o => o.IsActive && (!o.ExpiresAt.HasValue || o.ExpiresAt > DateTime.UtcNow));

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(o => o.Titre.Contains(search) || o.Description.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(zone))
        {
            query = query.Where(o => o.Zone.Contains(zone));
        }

        if (type.HasValue)
        {
            query = query.Where(o => o.Type == type.Value);
        }

        return await query.OrderByDescending(o => o.PublieeAt).ToListAsync();
    }

    public Task<Offre?> GetByIdAsync(int id)
    {
        return _context.Offres
            .Include(o => o.Recruteur)
            .Include(o => o.Candidatures)
            .ThenInclude(c => c.Candidat)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public Task<Offre?> GetByIdAndRecruiterAsync(int id, string recruiterId)
    {
        return _context.Offres
            .Include(o => o.Recruteur)
            .FirstOrDefaultAsync(o => o.Id == id && o.RecruteurId == recruiterId);
    }

    public Task<List<Offre>> GetByRecruiterAsync(string recruiterId)
    {
        return _context.Offres
            .AsNoTracking()
            .Where(o => o.RecruteurId == recruiterId)
            .OrderByDescending(o => o.PublieeAt)
            .ToListAsync();
    }

    public Task<int> CountActiveAsync()
    {
        return _context.Offres.CountAsync(o => o.IsActive && (!o.ExpiresAt.HasValue || o.ExpiresAt > DateTime.UtcNow));
    }

    public Task<int> CountByRecruiterAsync(string recruiterId)
    {
        return _context.Offres.CountAsync(o => o.RecruteurId == recruiterId);
    }

    public async Task AddAsync(Offre offre)
    {
        _context.Offres.Add(offre);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Offre offre)
    {
        _context.Offres.Update(offre);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Offre offre)
    {
        _context.Offres.Remove(offre);
        await _context.SaveChangesAsync();
    }
}

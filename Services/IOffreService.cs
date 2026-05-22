using RecruitApp.Models;
using RecruitApp.Models.Enums;

namespace RecruitApp.Services;

// Offre service
public interface IOffreService
{
    Task<List<Offre>> GetPublicOffersAsync(string? search = null, string? zone = null, TypeOffre? type = null);

    Task<Offre?> GetByIdAsync(int id);

    Task<Offre?> GetByIdAndRecruiterAsync(int id, string recruiterId);

    Task<List<Offre>> GetByRecruiterAsync(string recruiterId);

    Task<int> CountAllAsync();

    Task<int> CountActiveAsync();

    Task<int> CountByRecruiterAsync(string recruiterId);

    Task AddAsync(Offre offre);

    Task UpdateAsync(Offre offre);

    Task DeleteAsync(Offre offre);
}

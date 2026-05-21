using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecruitApp.Models;

namespace RecruitApp.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Offre> Offres { get; set; } = default!;

    public DbSet<Candidature> Candidatures { get; set; } = default!;

    public DbSet<Entretien> Entretiens { get; set; } = default!;

    public DbSet<Document> Documents { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Offre>()
            .Property(o => o.SalaireMin)
            .HasPrecision(18, 2);

        builder.Entity<Offre>()
            .Property(o => o.SalaireMax)
            .HasPrecision(18, 2);

        builder.Entity<Offre>()
            .HasOne(o => o.Recruteur)
            .WithMany(u => u.Offres)
            .HasForeignKey(o => o.RecruteurId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Candidature>()
            .HasOne(c => c.Candidat)
            .WithMany(u => u.Candidatures)
            .HasForeignKey(c => c.CandidatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Candidature>()
            .HasOne(c => c.Offre)
            .WithMany(o => o.Candidatures)
            .HasForeignKey(c => c.OffreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Entretien>()
            .HasOne(e => e.Candidature)
            .WithOne(c => c.Entretien)
            .HasForeignKey<Entretien>(e => e.CandidatureId);
    }
}
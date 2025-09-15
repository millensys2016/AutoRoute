using Microsoft.EntityFrameworkCore;

namespace dicomAPIs.Data
{
    public class DicomDbContext : DbContext
    {
        public DicomDbContext(DbContextOptions<DicomDbContext> options) : base(options)
        {
        }

        public DbSet<DicomRecord> DicomRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DicomRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PatientID).HasMaxLength(100);
                entity.Property(e => e.PatientName).HasMaxLength(200);
                entity.Property(e => e.StudyInstanceUID).HasMaxLength(200);
                entity.Property(e => e.SeriesInstanceUID).HasMaxLength(200);
                entity.Property(e => e.SOPInstanceUID).HasMaxLength(200);
                entity.Property(e => e.Modality).HasMaxLength(50);
                entity.Property(e => e.SavedFilePath).HasMaxLength(500);
            });
        }
    }
}
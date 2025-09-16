using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Study> Studies { get; set; }
        public DbSet<Series> Serieses { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Person constraints
            modelBuilder.Entity<Person>(entity =>
            {
                // Unique constraints
                entity.HasIndex(p => p.Email).IsUnique();
                entity.HasIndex(p => p.MobileNumber).IsUnique();

                // Required properties with length constraints
                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnType("nvarchar(256)");

                entity.Property(p => p.Email)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnType("varchar(256)");

                entity.Property(p => p.DateOfBirth)
                    .IsRequired()
                    .HasColumnType("date");

                entity.Property(p => p.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .HasColumnType("varchar(15)");

                entity.Property(p => p.Role)
                    .IsRequired();

                // Audit fields
                entity.Property(p => p.CreatedAt)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(p => p.UpdatedAt)
                    .HasColumnType("datetime");
            });

            // Patient constraints
            modelBuilder.Entity<Patient>(entity =>
            {
                // Unique constraints
                entity.HasIndex(p => p.PatientId).IsUnique();

                // Required properties with length constraints
                entity.Property(p => p.PersonId)
                    .IsRequired();

                entity.Property(p => p.PatientId)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnType("varchar(64)");

                entity.Property(p => p.PatientName)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnType("nvarchar(256)");

                entity.Property(p => p.PatientSex)
                    .IsRequired()
                    .HasMaxLength(1)
                    .HasColumnType("char(1)");

                entity.Property(p => p.PatientBirthDate)
                    .IsRequired()
                    .HasColumnType("date");

                entity.Property(p => p.PatientSize)
                    .HasColumnType("bigint");

                entity.Property(p => p.PatientWeight)
                    .HasColumnType("bigint");

                // Check constraint
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Patient_PatientSex",
                    "PatientSex IN ('M', 'F')"));

                // Add positive value constraints
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Patient_PatientSize",
                    "PatientSize IS NULL OR PatientSize >= 0"));

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Patient_PatientWeight",
                    "PatientWeight IS NULL OR PatientWeight >= 0"));

                // Audit fields
                entity.Property(p => p.CreatedAt)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(p => p.UpdatedAt)
                    .HasColumnType("datetime");

                // One-to-one relationship with Person
                entity.HasOne(p => p.Person)
                      .WithOne()
                      .HasForeignKey<Patient>(p => p.PersonId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Study constraints
            modelBuilder.Entity<Study>(entity =>
            {
                // Required properties with length constraints
                entity.Property(s => s.PatientId)
                    .IsRequired();

                entity.Property(s => s.StudyInstanceUID)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnType("varchar(64)");

                entity.Property(s => s.StudyDate)
                    .IsRequired()
                    .HasColumnType("date");

                entity.Property(s => s.StudyDescription)
                    .HasMaxLength(64)
                    .HasColumnType("varchar(256)");

                entity.Property(s => s.StudyTime)
                    .IsRequired()
                    .HasColumnType("time");

                entity.Property(s => s.StudyId)
                    .HasColumnType("bigint");

                // Add positive value constraint
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Study_StudyId",
                    "StudyId IS NULL OR StudyId >= 0"));

                // Audit fields
                entity.Property(s => s.CreatedAt)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(s => s.UpdatedAt)
                    .HasColumnType("datetime");

                // One-to-many relationship with Patient
                entity.HasOne(s => s.Patient)
                      .WithMany(p => p.Studies)
                      .HasForeignKey(s => s.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Series constraints
            modelBuilder.Entity<Series>(entity =>
            {
                // Required properties with length constraints
                entity.Property(s => s.StudyId)
                    .IsRequired();

                entity.Property(s => s.SeriesInstanceUID)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnType("varchar(64)");

                entity.Property(s => s.SeriesNumber)
                    .IsRequired()
                    .HasColumnType("bigint");

                entity.Property(s => s.Modality)
                    .IsRequired()
                    .HasMaxLength(16)
                    .HasColumnType("varchar(32)");

                entity.Property(s => s.SeriesDescription)
                    .HasMaxLength(64)
                    .HasColumnType("varchar(64)");

                entity.Property(s => s.SeriesTime)
                    .IsRequired()
                    .HasColumnType("time");

                // Add positive value constraint
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Series_SeriesNumber",
                    "SeriesNumber >= 0"));

                // Audit fields
                entity.Property(s => s.CreatedAt)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(s => s.UpdatedAt)
                    .HasColumnType("datetime");

                // One-to-many relationship with Study
                entity.HasOne(s => s.Study)
                      .WithMany(s => s.Series)
                      .HasForeignKey(s => s.StudyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Image constraints
            modelBuilder.Entity<Image>(entity =>
            {
                // Required properties with length constraints
                entity.Property(i => i.SeriesId)
                    .IsRequired();

                entity.Property(i => i.SOPInstanceUID)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnType("varchar(64)");

                entity.Property(i => i.SOPClassUID)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnType("varchar(64)");

                entity.Property(i => i.InstanceNumber)
                    .IsRequired()
                    .HasColumnType("bigint");

                entity.Property(i => i.Rows)
                    .IsRequired()
                    .HasColumnType("bigint");

                entity.Property(i => i.Columns)
                    .IsRequired()
                    .HasColumnType("bigint");

                entity.Property(i => i.BitsAllocated)
                    .IsRequired()
                    .HasColumnType("bigint");

                entity.Property(i => i.AcquisitionDate)
                    .HasColumnType("date");

                entity.Property(i => i.AcquisitionTime)
                    .HasColumnType("time");

                // Add positive value constraints
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Image_InstanceNumber",
                    "InstanceNumber >= 0"));

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Image_Rows",
                    "Rows >= 0"));

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Image_Columns",
                    "Columns >= 0"));

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Image_BitsAllocated",
                    "BitsAllocated >= 0"));

                // Audit fields
                entity.Property(i => i.CreatedAt)
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(i => i.UpdatedAt)
                    .HasColumnType("datetime");

                // One-to-many relationship with Series
                entity.HasOne(i => i.Series)
                      .WithMany(s => s.Images)
                      .HasForeignKey(i => i.SeriesId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Series")]
    public class Series
    {
        // Primary key: auto-incrementing integer starting from 1
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign key to Study (many-to-one relationship)
        [Required]
        public required int StudyId { get; set; }

        // Navigation property to Study
        [ForeignKey("StudyId")]
        public required Study Study { get; set; }

        // Series Instance UID from DICOM
        [Required]
        [MaxLength(64)]
        [Column(TypeName = "varchar(64)")]
        public required string SeriesInstanceUID { get; set; }

        // Series Number from DICOM
        [Required]
        [Range(1, uint.MaxValue, ErrorMessage = "Series number must be a positive integer.")]
        public required uint SeriesNumber { get; set; }

        // Modality from DICOM (CT, MR, US, etc.)
        [Required]
        [MaxLength(32)]
        [Column(TypeName = "varchar(32)")]
        public required string Modality { get; set; }

        // Series Description from DICOM
        [MaxLength(64)]
        [Column(TypeName = "varchar(256)")]
        public string? SeriesDescription { get; set; }

        // Series Time from DICOM
        [Required]
        [Column(TypeName = "time")]
        public required TimeOnly SeriesTime { get; set; }

        // CreatedAt: datetime with default value now
        [Required]
        [Column(TypeName = "datetime")]
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // UpdatedAt: datetime, nullable
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Image> Images = [];
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Studies")]
    public class Study
    {
        // Primary key: auto-incrementing integer starting from 1
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign key to Patient
        [Required]
        public required int PatientId { get; set; }

        // Navigation property to Patient
        [ForeignKey("PatientId")]
        public required Patient Patient { get; set; }

        // Study Instance UID from DICOM
        [Required]
        [MaxLength(64)]
        [Column(TypeName = "varchar(64)")]
        public required string StudyInstanceUID { get; set; }

        // Study Date from DICOM
        [Required]
        [Column(TypeName = "date")]
        public required DateOnly StudyDate { get; set; }

        // Study Description from DICOM
        [MaxLength(64)]
        [Column(TypeName = "varchar(256)")]
        public string? StudyDescription { get; set; }

        // Study Time from DICOM
        [Required]
        [Column(TypeName = "time")]
        public required TimeOnly StudyTime { get; set; }

        // Study ID from DICOM
        public uint? StudyId { get; set; }

        // CreatedAt: datetime with default value now
        [Required]
        [Column(TypeName = "datetime")]
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // UpdatedAt: datetime, nullable
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Series> Series { get; set; } = [];
    }
}
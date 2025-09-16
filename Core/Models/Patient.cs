using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Patients")]
    public class Patient
    {
        // Primary key: auto-incrementing integer starting from 1
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign key to Person
        [Required]
        public required int PersonId { get; set; }

        // Navigation property to Person
        [ForeignKey("PersonId")]
        public required Person Person { get; set; }

        // Patient ID from DICOM
        [Required]
        [MaxLength(64)]
        [Column(TypeName = "varchar(64)")]
        public required string PatientId { get; set; }

        // Patient Name from DICOM
        [Required]
        [MaxLength(256)]
        [Column(TypeName = "nvarchar(256)")]
        public required string PatientName { get; set; }

        // Patient Sex from DICOM
        [Required]
        [MaxLength(1)]
        [RegularExpression("^(M|F)$", ErrorMessage = "Patient sex must be 'M' or 'F'.")]
        [Column(TypeName = "char(1)")]
        public required string PatientSex { get; set; }

        // Patient Birth Date from DICOM
        [Required]
        [Column(TypeName = "date")]
        public required DateOnly PatientBirthDate { get; set; }

        // Patient Size from DICOM (in cm)
        public required uint? PatientSize { get; set; }

        // Patient Weight from DICOM (in kg)
        public required uint? PatientWeight { get; set; }

        // CreatedAt: datetime with default value now
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // UpdatedAt: datetime, nullable
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Study> Studies { get; set; } = [];
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("Images")]
    public class Image
    {
        // Primary key: auto-incrementing integer starting from 1
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign key to Series (many-to-one relationship)
        [Required]
        public int SeriesId { get; set; }

        // Navigation property to Series
        [ForeignKey("SeriesId")]
        public required Series Series { get; set; }

        // SOP Instance UID from DICOM (unique identifier for this image)
        [Required]
        [MaxLength(64)]
        [Column(TypeName = "varchar(64)")]
        public string SOPInstanceUID { get; set; } = string.Empty;

        // Instance Number from DICOM (image number within the series)
        [Required]
        [Range(1, uint.MaxValue, ErrorMessage = "Instance number must be a positive integer.")]
        public required uint InstanceNumber { get; set; }

        // Image dimensions - Rows (height in pixels)
        [Required]
        [Range(1, uint.MaxValue, ErrorMessage = "Image rows must be a positive integer.")]
        public required uint Rows { get; set; }

        // Image dimensions - Columns (width in pixels)
        [Required]
        [Range(1, uint.MaxValue, ErrorMessage = "Image columns must be a positive integer.")]
        public required uint Columns { get; set; }

        // Bits Allocated per pixel from DICOM
        [Required]
        [Range(1, uint.MaxValue, ErrorMessage = "Bits allocated must be a positive integer.")]
        public uint BitsAllocated { get; set; }

        // SOP Class UID from DICOM (defines the type of DICOM object)
        [Required]
        [MaxLength(64)]
        [Column(TypeName = "varchar(64)")]
        public required string SOPClassUID { get; set; }

        // Acquisition Date from DICOM
        [Column(TypeName = "date")]
        public DateOnly? AcquisitionDate { get; set; }

        // Acquisition Time from DICOM
        [Column(TypeName = "time")]
        public TimeOnly? AcquisitionTime { get; set; }

        // File path on the local server
        [Required, MaxLength(512)]
        [Column(TypeName = "varchar(512)")]
        public required string Path { get; set; }

        // CreatedAt: datetime with default value now
        [Required]
        [Column(TypeName = "datetime")]
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // UpdatedAt: datetime, nullable
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
    }
}
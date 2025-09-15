using System.ComponentModel.DataAnnotations;

namespace dicomAPIs.Data
{
    public class DicomRecord
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string? PatientID { get; set; }

        [MaxLength(200)]
        public string? PatientName { get; set; }

        [MaxLength(200)]
        public string? StudyInstanceUID { get; set; }

        [MaxLength(200)]
        public string? SeriesInstanceUID { get; set; }

        [MaxLength(200)]
        public string? SOPInstanceUID { get; set; }

        [MaxLength(50)]
        public string? Modality { get; set; }

        public DateTime StudyDate { get; set; }

        [MaxLength(500)]
        public string SavedFilePath { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

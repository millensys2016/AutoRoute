namespace Core.DTOs.Upload
{
    public class UploadDicomFileResponse
    {
        public required PatientDto Patient { get; set; }
        public required StudyDto Study { get; set; } 
        public required SeriesDto Series { get; set; } 
        public required ImageDto Image { get; set; } 
        public required string Path { get; set; }
    }

    public class PatientDto
    {
        public string PatientId { get; set; } = "Unknown";
        public string PatientName { get; set; } = "Unknown";
        public string PatientSex { get; set; } = "Unknown";
        public string PatientBirthDate { get; set; } = "Unknown";
    }

    public class StudyDto
    {
        public string StudyInstanceUID { get; set; } = "Unknown";
        public string StudyDate { get; set; } = "Unknown";
        public string StudyDescription { get; set; } = "Unknown";
    }

    public class SeriesDto
    {
        public string SeriesInstanceUID { get; set; } = "Unknown";
        public uint SeriesNumber { get; set; }
        public string Modality { get; set; } = "Unknown";
    }

    public class ImageDto
    {
        public string SOPInstanceUID { get; set; } = "Unknown";
        public uint InstanceNumber { get; set; }
        public uint Rows { get; set; }
        public uint Columns { get; set; }
        public uint BitsAllocated { get; set; }
    }
}

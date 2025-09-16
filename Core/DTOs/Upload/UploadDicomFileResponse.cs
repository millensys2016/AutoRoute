using System.Formats.Asn1;

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
        public DateOnly PatientBirthDate { get; set; }
        public uint PatientSize { get; set; }
        public uint PatientWeight { get; set; }
    }

    public class StudyDto
    {
        public string StudyInstanceUID { get; set; } = "Unknown";
        public DateOnly StudyDate { get; set; }
        public string StudyDescription { get; set; } = "Unknown";
        public TimeOnly StudyTime { get; set; }
        public uint StudyId { get; set; }
    }

    public class SeriesDto
    {
        public string SeriesInstanceUID { get; set; } = "Unknown";
        public uint SeriesNumber { get; set; }
        public string Modality { get; set; } = "Unknown";
        public string SeriesDescription { get; set; } = "Unknown";
        public TimeOnly SeriesTime { get; set; }
    }

    public class ImageDto
    {
        public string SOPInstanceUID { get; set; } = "Unknown";
        public uint InstanceNumber { get; set; }
        public uint Rows { get; set; }
        public uint Columns { get; set; }
        public uint BitsAllocated { get; set; }
        public string SOPClassUID { get; set; } = "Unknown";
        public DateOnly AcquisitionDate { get; set; }
        public TimeOnly AcquisitionTime {  get; set; }
    }
}

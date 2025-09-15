namespace dicomAPIs.DTO
{
    public record DicomDTO
    {
        public string? PatientID { get; init; }
        public string? PatientName { get; init; }
        public string? StudyInstanceUID { get; init; }
        public string? SeriesInstanceUID { get; init; }
        public string? SOPInstanceUID { get; init; }
        public string? Modality { get; init; }
        public string? StudyDate { get; init; }
    }
}

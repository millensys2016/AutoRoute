namespace dicomAPIs.DTO
{
    public class DicomDataDTO
    {
        public DicomDTO DicomData { get; init; } = new();
        public string OriginalFileName { get; init; } = string.Empty;
        public List<DicomTagDTO> AllTags { get; init; } = new();
    }
}

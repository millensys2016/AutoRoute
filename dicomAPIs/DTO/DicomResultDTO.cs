namespace dicomAPIs.DTO
{
    public record DicomResultDTO
    {
        public DicomDTO Metadata { get; init; } = new();
        public string SavedPath { get; init; } = string.Empty;
     }
}

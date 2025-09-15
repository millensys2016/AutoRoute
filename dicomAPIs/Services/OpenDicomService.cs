using dicomAPIs.Data;
using dicomAPIs.DTO;
using FellowOakDicom;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace dicomAPIs.Services
{
    public class OpenDicomService
    {
        private readonly DicomDbContext _context;
        private readonly ILogger<OpenDicomService> _logger;
        private readonly string _outputDirectory;

        public OpenDicomService(DicomDbContext context, ILogger<OpenDicomService> logger)
        {
            _context = context;
            _logger = logger;
            _outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "DicomOutputs");

            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }
        }

        public async Task<DicomDataDTO> ExtractDicomMetadataAsync(IFormFile formFile)
        {
            try
            {
                _logger.LogInformation("Extracting DICOM metadata from file: {FileName}", formFile.FileName);

                using var stream = formFile.OpenReadStream();
                var dicomFile = DicomFile.Open(stream);

                var dicomDto = ExtractDicomMetadata(dicomFile);
                var allTags = ExtractAllDicomTags(dicomFile);

                _logger.LogInformation("DICOM metadata extracted successfully from file: {FileName}", formFile.FileName);

                return new DicomDataDTO
                {
                    DicomData = dicomDto,
                    OriginalFileName = formFile.FileName,
                    AllTags = allTags
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting DICOM metadata from file: {FileName}", formFile.FileName);
                throw;
            }
        }

        public async Task<DicomResultDTO> SaveDicomDataAsync(DicomDataDTO dicomData)
        {
            try
            {
                _logger.LogInformation("Saving DICOM data for file: {FileName}", dicomData.OriginalFileName);


                var filePath = await SaveDicomTagsToFileAsync(dicomData.AllTags, dicomData.OriginalFileName);

                await SaveToDatabase(dicomData.DicomData, filePath);

                _logger.LogInformation("DICOM data saved successfully. Output saved to: {FilePath}", filePath);

                return new DicomResultDTO
                {
                    Metadata = dicomData.DicomData,
                    SavedPath = filePath
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving DICOM data for file: {FileName}", dicomData.OriginalFileName);
                throw;
            }
        }


        public async Task<DicomResultDTO> ProcessDicomFileAsync(IFormFile formFile)
        {
            try
            {
                _logger.LogInformation("Processing DICOM file: {FileName}", formFile.FileName);

                using var stream = formFile.OpenReadStream();
                var dicomFile = DicomFile.Open(stream);

                var dicomDto = ExtractDicomMetadata(dicomFile);
                var allTags = ExtractAllDicomTags(dicomFile);
                var filePath = await SaveDicomTagsToFileAsync(allTags, formFile.FileName);

                await SaveToDatabase(dicomDto, filePath);

                _logger.LogInformation("DICOM file processed successfully. Output saved to: {FilePath}", filePath);

                return new DicomResultDTO
                {
                    Metadata = dicomDto,
                    SavedPath = filePath
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing DICOM file: {FileName}", formFile.FileName);
                throw;
            }
        } // old method

        private DicomDTO ExtractDicomMetadata(DicomFile dicomFile)
        {
            var dataset = dicomFile.Dataset;

            return new DicomDTO
            {
                PatientID = dataset.GetSingleValueOrDefault(DicomTag.PatientID, string.Empty),
                PatientName = dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty),
                StudyInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty),
                SeriesInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty),
                SOPInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, string.Empty),
                Modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, string.Empty),
                StudyDate = dataset.GetSingleValueOrDefault(DicomTag.StudyDate, string.Empty)
            };
        }


        private List<DicomTagDTO> ExtractAllDicomTags(DicomFile dicomFile)
        {
            var tags = new List<DicomTagDTO>();

            foreach (var element in dicomFile.Dataset)
            {
                var value = dicomFile.Dataset.GetValueOrDefault(element.Tag, 0, string.Empty);
                tags.Add(new DicomTagDTO
                {
                    Tag = element.Tag.ToString(),
                    Name = element.Tag.DictionaryEntry.Name,
                    Value = value?.ToString() ?? string.Empty
                });
            }

            return tags;
        }
        private async Task<string> SaveDicomTagsToFileAsync(List<DicomTagDTO> allTags, string originalFileName)
        {
            var fileName = $"DICOM_Tags_{Path.GetFileNameWithoutExtension(originalFileName)}.txt";
            var filePath = Path.Combine(_outputDirectory, fileName);

            var content = new StringBuilder();
            content.AppendLine($"DICOM Tags Report");
            content.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            content.AppendLine($"Original File: {originalFileName}");
            content.AppendLine(new string('=', 60));
            content.AppendLine();

            foreach (var tag in allTags)
            {
                content.AppendLine($"{tag.Tag} | {tag.Name} | {tag.Value}");
            }

            await File.WriteAllTextAsync(filePath, content.ToString());
            return filePath;
        }

        private async Task SaveToDatabase(DicomDTO dicomDto, string filePath)
        {
            var dicomRecord = new DicomRecord
            {
                PatientID = dicomDto.PatientID,
                PatientName = dicomDto.PatientName,
                StudyInstanceUID = dicomDto.StudyInstanceUID,
                SeriesInstanceUID = dicomDto.SeriesInstanceUID,
                SOPInstanceUID = dicomDto.SOPInstanceUID,
                Modality = dicomDto.Modality,
                StudyDate = ParseStudyDate(dicomDto.StudyDate),
                SavedFilePath = filePath,
                CreatedAt = DateTime.UtcNow
            };

            _context.DicomRecords.Add(dicomRecord);
            await _context.SaveChangesAsync();
        }

        private DateTime ParseStudyDate(string? studyDate)
        {
            if (string.IsNullOrEmpty(studyDate))
                return DateTime.MinValue;

            // DICOM date format is YYYYMMDD
            if (DateTime.TryParseExact(studyDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            return DateTime.MinValue;
        }
    }
}
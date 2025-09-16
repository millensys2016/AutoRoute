using dicomAPIs.Data;
using dicomAPIs.DTO;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.Render;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
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
                var fileFolder = CreateFileFolderStructure(formFile.FileName);
                await SaveOriginalDicomFileAsync(formFile, fileFolder);

                _logger.LogInformation("DICOM metadata extracted successfully from file: {FileName}", formFile.FileName);

                return new DicomDataDTO
                {
                    DicomData = dicomDto,
                    OriginalFileName = formFile.FileName,
                    AllTags = allTags,
                    FileFolder = fileFolder
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

                var filePath = await SaveDicomTagsToFileAsync(dicomData.AllTags, dicomData.OriginalFileName, dicomData.FileFolder);
                await SaveDicomImageAsync(dicomData.OriginalFileName,dicomData.FileFolder);
                await SaveToDatabase(dicomData.DicomData, dicomData.FileFolder);

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

        private string CreateFileFolderStructure(string originalFileName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var folderName = $"{fileNameWithoutExtension}_{timestamp}";
            
            var fileFolder = Path.Combine(_outputDirectory, folderName);
            
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }
            
            _logger.LogInformation("Created output folder: {FolderPath}", fileFolder);
            return fileFolder;
        }

        private async Task SaveOriginalDicomFileAsync(IFormFile formFile, string outputFolder)
        {
            try
            {
                var originalFilePath = Path.Combine(outputFolder, formFile.FileName);
                
                using var fileStream = new FileStream(originalFilePath, FileMode.Create);
                await formFile.CopyToAsync(fileStream);
                
                _logger.LogInformation("Original DICOM file saved to: {FilePath}", originalFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to save original DICOM file");
            }
        }

        private async Task SaveDicomImageAsync(string originalFileName, string outputFolder)
        {
            try
            {
                var filePath = Path.Combine(outputFolder, originalFileName);
                var dicomFile = DicomFile.Open(filePath);
                if (!dicomFile.Dataset.Contains(DicomTag.PixelData))
                {
                    _logger.LogInformation("No pixel data found in DICOM file: {FileName}", originalFileName);
                    return;
                }

                var image = new DicomImage(dicomFile.Dataset);
                
                var renderedImage = image.RenderImage();
                
                var width = renderedImage.Width;
                var height = renderedImage.Height;
                var pixelData = renderedImage.Pixels;
                
                using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                
                var bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

                try
                {
                    var stride = bitmapData.Stride;
                    var totalBytes = Math.Abs(stride) * height;

                    var sourceBytes = new byte[totalBytes];
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pointer, sourceBytes, 0, totalBytes);

                    System.Runtime.InteropServices.Marshal.Copy(sourceBytes, 0, bitmapData.Scan0, totalBytes);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                
                var imageFileName = $"{Path.GetFileNameWithoutExtension(originalFileName)}_image.png";
                var imagePath = Path.Combine(outputFolder, imageFileName);
                
                bitmap.Save(imagePath, ImageFormat.Png);
                
                _logger.LogInformation("DICOM image extracted and saved to: {ImagePath}", imagePath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract image from DICOM file: {FileName}", originalFileName);
            }
        }

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

        private async Task<string> SaveDicomTagsToFileAsync(List<DicomTagDTO> allTags, string originalFileName, string outputFolder)
        {
            var fileName = $"DICOM_Tags_{Path.GetFileNameWithoutExtension(originalFileName)}.txt";
            var filePath = Path.Combine(outputFolder, fileName);

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
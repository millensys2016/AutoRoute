using Core.DTOs.Upload;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Dicom;

namespace APIs.Services
{
    public class UploadService(IBaseRepository<Person> personRepository, 
        IBaseRepository<Patient> patientRepository, 
        IBaseRepository<Study> studyRepository, 
        IBaseRepository<Series> seriesRepository, 
        IBaseRepository<Image> imageRepository) : IUploadService
    {
        private readonly IBaseRepository<Person> _personRepository = personRepository;
        private readonly IBaseRepository<Patient> _patientRepository = patientRepository;
        private readonly IBaseRepository<Study> _studyRepository = studyRepository;
        private readonly IBaseRepository<Series> _seriesRepository = seriesRepository;
        private readonly IBaseRepository<Image> _imageRepository = imageRepository;

        public async Task<UploadDicomFileResponse> ExtractDicomFileInfo(UploadDicomFileRequest request)
        {
            var stream = request.DicomFile.OpenReadStream();
            var dicom = await DicomFile.OpenAsync(stream);
            var dataset = dicom.Dataset;

            var response = new UploadDicomFileResponse
            {
                Patient = new PatientDto
                {
                    PatientId = dataset.GetSingleValueOrDefault(DicomTag.PatientID, "Unknown"),
                    PatientName = dataset.GetSingleValueOrDefault(DicomTag.PatientName, "Unknown"),
                    PatientSex = dataset.GetSingleValueOrDefault(DicomTag.PatientSex, "Unknown"),
                    PatientBirthDate = dataset.GetSingleValueOrDefault(DicomTag.PatientBirthDate, DateOnly.MinValue),
                    PatientSize = (uint)dataset.GetSingleValueOrDefault(DicomTag.PatientSize, 0),
                    PatientWeight = (uint)dataset.GetSingleValueOrDefault(DicomTag.PatientWeight, 0),
                },
                Study = new StudyDto
                {
                    StudyInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "Unknown"),
                    StudyDate = dataset.GetSingleValueOrDefault(DicomTag.StudyDate, DateOnly.MinValue),
                    StudyDescription = dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, "Unknown"),
                    StudyTime = dataset.GetSingleValueOrDefault(DicomTag.StudyTime, TimeOnly.MinValue),
                    StudyId = (uint)dataset.GetSingleValueOrDefault(DicomTag.StudyID, 0),
                },
                Series = new SeriesDto
                {
                    SeriesInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, "Unknown"),
                    SeriesNumber = (uint)dataset.GetSingleValueOrDefault(DicomTag.SeriesNumber, 0),
                    Modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, "Unknown"),
                    SeriesTime = dataset.GetSingleValueOrDefault(DicomTag.SeriesTime, TimeOnly.MinValue),
                    SeriesDescription = dataset.GetSingleValueOrDefault(DicomTag.SeriesDescription, "Unkown"),
                },
                Image = new ImageDto
                {
                    SOPInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, "Unknown"),
                    SOPClassUID = dataset.GetSingleValueOrDefault(DicomTag.SOPClassUID, "Unknown"),
                    AcquisitionDate = dataset.GetSingleValueOrDefault(DicomTag.AcquisitionDate, DateOnly.MinValue),
                    AcquisitionTime = dataset.GetSingleValueOrDefault(DicomTag.AcquisitionTime, TimeOnly.MinValue),
                    InstanceNumber = (uint)dataset.GetSingleValueOrDefault(DicomTag.InstanceNumber, 0),
                    Rows = (uint)dataset.GetSingleValueOrDefault(DicomTag.Rows, 0),
                    Columns = (uint)dataset.GetSingleValueOrDefault(DicomTag.Columns, 0),
                    BitsAllocated = (uint)dataset.GetSingleValueOrDefault(DicomTag.BitsAllocated, 0)
                },

                Path = await SaveDicomFileToDisk(request.DicomFile, dataset.GetSingleValueOrDefault(DicomTag.PatientID, "Unknown"))
            };

            return response;
        }

        public async Task<string> SaveDicomFileToDisk(IFormFile dicomFile, string patientId)
        {
            // Sanitize patient ID for folder name (remove invalid characters)
            var sanitizedPatientId = string.Join("_", patientId.Split(Path.GetInvalidFileNameChars()));

            // Create the folder path
            var folderPath = Path.Combine("Dicom Images", "Patients", sanitizedPatientId);

            // Ensure directory exists
            Directory.CreateDirectory(folderPath);

            // Generate filename with current timestamp
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileExtension = Path.GetExtension(dicomFile.FileName) ?? ".dcm";
            var fileName = $"{timestamp}{fileExtension}";

            // Full file path
            var filePath = Path.Combine(folderPath, fileName);

            // Save the file
            var fileStream = new FileStream(filePath, FileMode.Create);
            await dicomFile.CopyToAsync(fileStream);
            fileStream.Dispose();

            return filePath;
        }
    }
}
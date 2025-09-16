using Core.DTOs.Upload;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Dicom;
using Microsoft.EntityFrameworkCore;

namespace APIs.Services
{
    public class UploadService(IBaseRepository<Person> personRepository,
        IPatientRepository patientRepository,
        IBaseRepository<Study> studyRepository,
        IBaseRepository<Series> seriesRepository,
        IBaseRepository<Image> imageRepository) : IUploadService
    {
        private readonly IBaseRepository<Person> _personRepository = personRepository;
        private readonly IPatientRepository _patientRepository = patientRepository;
        private readonly IBaseRepository<Study> _studyRepository = studyRepository;
        private readonly IBaseRepository<Series> _seriesRepository = seriesRepository;
        private readonly IBaseRepository<Image> _imageRepository = imageRepository;

        public async Task<UploadDicomFileResponse?> ExtractDicomFileInfo(UploadDicomFileRequest request)
        {
            try
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

                    Path = await SaveDicomFileToDisk(request.DicomFile, dataset.GetSingleValueOrDefault(DicomTag.PatientID, "Unknown")) ?? ""
                };

                if (response.Path == "")
                    return null;

                bool? savingDicomFileToDatabaseFlag = await SaveDicomFileToDatabase(response);

                if (savingDicomFileToDatabaseFlag == null || savingDicomFileToDatabaseFlag == false)
                    return null;


                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<string?> SaveDicomFileToDisk(IFormFile dicomFile, string patientId)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool?> SaveDicomFileToDatabase(UploadDicomFileResponse response)
        {
            try
            {
                var existingPatient = await _patientRepository.GetPatientByPatientId(response.Patient.PatientId);

                if (existingPatient == null)
                {
                    var person = new Person
                    {
                        Name = response.Patient.PatientName,
                        Email = $"patient_{response.Patient.PatientId}@example.com",
                        DateOfBirth = response.Patient.PatientBirthDate,
                        MobileNumber = $"+201{Random.Shared.Next(100000000, 999999999)}",
                        Role = PersonRole.Patient
                    };

                    var createdPerson = await _personRepository.Add(person);
                    if (createdPerson == null)
                    {
                        return false;
                    }

                    var patient = new Patient
                    {
                        PersonId = createdPerson.Id,
                        Person = createdPerson,
                        PatientId = response.Patient.PatientId,
                        PatientName = response.Patient.PatientName,
                        PatientSex = response.Patient.PatientSex,
                        PatientBirthDate = response.Patient.PatientBirthDate,
                        PatientSize = response.Patient.PatientSize == 0 ? null : response.Patient.PatientSize,
                        PatientWeight = response.Patient.PatientWeight == 0 ? null : response.Patient.PatientWeight,
                    };

                    existingPatient = await _patientRepository.Add(patient);
                    if (existingPatient == null)
                    {
                        return false;
                    }
                }

                var study = new Study
                {
                    PatientId = existingPatient.Id,
                    Patient = existingPatient,
                    StudyInstanceUID = response.Study.StudyInstanceUID,
                    StudyDate = response.Study.StudyDate,
                    StudyTime = response.Study.StudyTime,
                    StudyDescription = response.Study.StudyDescription == "Unknown" ? null : response.Study.StudyDescription,
                    StudyId = response.Study.StudyId == 0 ? null : response.Study.StudyId,
                };

                var createdStudy = await _studyRepository.Add(study);
                if (createdStudy == null)
                {
                    return false;
                }

                // Create new series
                var series = new Series
                {
                    StudyId = createdStudy.Id,
                    Study = createdStudy,
                    SeriesInstanceUID = response.Series.SeriesInstanceUID,
                    SeriesNumber = response.Series.SeriesNumber,
                    Modality = response.Series.Modality,
                    SeriesTime = response.Series.SeriesTime,
                    SeriesDescription = response.Series.SeriesDescription == "Unknown" ? null : response.Series.SeriesDescription,
                };

                var createdSeries = await _seriesRepository.Add(series);
                if (createdSeries == null)
                {
                    return false;
                }

                // Create new image
                var image = new Image
                {
                    SeriesId = createdSeries.Id,
                    Series = createdSeries,
                    SOPInstanceUID = response.Image.SOPInstanceUID,
                    SOPClassUID = response.Image.SOPClassUID,
                    InstanceNumber = response.Image.InstanceNumber,
                    Rows = response.Image.Rows,
                    Columns = response.Image.Columns,
                    BitsAllocated = response.Image.BitsAllocated,
                    AcquisitionDate = response.Image.AcquisitionDate == DateOnly.MinValue ? null : response.Image.AcquisitionDate,
                    AcquisitionTime = response.Image.AcquisitionTime == TimeOnly.MinValue ? null : response.Image.AcquisitionTime,
                    Path = response.Path,
                };

                var createdImage = await _imageRepository.Add(image);
                return createdImage != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
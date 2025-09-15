using Core.DTOs.Upload;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Services
{
    public interface IUploadService
    {
        Task<UploadDicomFileResponse> ExtractDicomFileInfo(UploadDicomFileRequest request);
        Task<string> SaveDicomFileToDisk(IFormFile dicomFile, string patientId);
    }
}

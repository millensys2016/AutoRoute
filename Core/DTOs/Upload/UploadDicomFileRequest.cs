using Microsoft.AspNetCore.Http;

namespace Core.DTOs.Upload
{
    public class UploadDicomFileRequest
    {
        public required IFormFile DicomFile { get; set; }
    }
}
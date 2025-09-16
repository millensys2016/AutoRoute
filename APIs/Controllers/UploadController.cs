using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Services;
using Core.DTOs.Upload;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController(IUploadService uploadService) : ControllerBase
    {
        private readonly IUploadService _uploadService = uploadService;

        [HttpPost]
        [Route("dicom")]
        public async Task<ActionResult<UploadDicomFileResponse>> ExtractDicomInfo([FromForm] UploadDicomFileRequest request)
        {
            if (request.DicomFile is null || request.DicomFile.Length == 0)
                return BadRequest("No Dicom file provided");

            UploadDicomFileResponse? response = await _uploadService.ExtractDicomFileInfo(request);

            if (response == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok(response);
        }


    }
}

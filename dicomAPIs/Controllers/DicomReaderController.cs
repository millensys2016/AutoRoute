using dicomAPIs.DTO;
using dicomAPIs.Services;
using Microsoft.AspNetCore.Mvc;

namespace dicomAPIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DicomReaderController : ControllerBase
    {
        private readonly OpenDicomService _dicomService;

        public DicomReaderController(OpenDicomService dicomService)
        {
            _dicomService = dicomService;
        }
        //[HttpPost]
        //public async Task<IActionResult> uploadDicomFile(IFormFile file)
        //{
        //    if (file is null || file.Length == 0) 
        //        return BadRequest("No file uploaded.");
        //    try
        //    {
        //        var result = await _dicomService.ProcessDicomFileAsync(file);
        //        return Ok(result);
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, $"Error occurred during DICOM file processing: {e.Message}");
        //    }
        //}

        [HttpPost("extractData")]
        public async Task<IActionResult> ExtractDicomMetadata(IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var metadata = await _dicomService.ExtractDicomMetadataAsync(file);
                return Ok(metadata);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error occurred during DICOM metadata extraction: {e.Message}");
            }
        }

        [HttpPost("SaveData")]
        public async Task<IActionResult> SaveDicomData([FromBody] DicomDataDTO request)
        {
            if (request.DicomData == null || request.AllTags == null || string.IsNullOrEmpty(request.OriginalFileName))
                return BadRequest("Invalid request data.");

            try
            {
                var result = await _dicomService.SaveDicomDataAsync(request);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error occurred during DICOM data saving: {e.Message}");
            }
        }

    }
}

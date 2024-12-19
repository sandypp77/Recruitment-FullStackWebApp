using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment_FullStackWebApp.Services;

namespace Recruitment_FullStackWebApp.Controllers
{
    [Route("resume")]
    public class FileController : Controller
    {
        private readonly IUserService _userService;
        private readonly IApplicantService _applicantService;
        private readonly IFileService _fileService;

        public FileController(IApplicantService applicantService, IUserService userService, IFileService fileService)
        {
            _userService = userService;
            _applicantService = applicantService;
            _fileService = fileService;
        }


        /*
         * Route: /{applicantId}
         * Returns the resume file for the specified applicant ID, or a not found/error message if issues occur.
         */
        [HttpGet("{applicantId}")]
        public IActionResult GetResume(int applicantId)
        {
            try
            {
                // Get the file path and content type (e.g., PDF, DOCX)
                var fileData = _fileService.GetResumeFile(applicantId);

                if (fileData == null)
                    return NotFound(new { success = false, message = "Resume not found for this applicant." });

                // Return the file as a download
                return File(fileData.FileStream, fileData.ContentType, fileData.FileName);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "You are not authorized to access this file." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}

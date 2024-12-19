using Recruitment_FullStackWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Services;

namespace Recruitment_FullStackWebApp.Controllers
{
    [Route("applicant")]
    [Authorize]
    public class ApplicantController : Controller
    {
        private readonly IUserService _userService;
        private readonly IApplicantService _applicantService;

        public ApplicantController(IApplicantService applicantService, IUserService userService)
        {
            _userService = userService;
            _applicantService = applicantService;
        }

        /*
         * Route: /
         * Displays a paginated list of jobs based on search filters (title, location).
         */
        [AllowAnonymous]
        public IActionResult Index(string title, string location, int pageNumber = 1, int pageSize = 10)
        {
            var paginatedJobs = _applicantService.GetAllJobs(title, location, pageNumber, pageSize);

            var viewModel = new JobViewModel
            {
                Jobs = paginatedJobs.Items,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling((double)paginatedJobs.TotalCount / pageSize),
                TotalItems = paginatedJobs.TotalCount,
                ItemsPerPage = pageSize,
                Message = !paginatedJobs.Items.Any() ? "No jobs available at the moment." : null
            };

            return View("~/Views/Applicant/JobListing/Index.cshtml", viewModel);
        }

        /*
         * Route: /profile
         * Retrieves and displays the applicant’s profile. If not found, shows the profile creation form.
         */
        [AllowAnonymous]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var email = Request.Cookies["userEmail"];
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var user = _userService.GetUserByEmail(email);
            var applicant = _applicantService.GetApplicantProfile(user.Id);
            if (applicant == null)
            {
                return View("~/Views/Applicant/Profile/Create.cshtml");
            }

            var viewModel = new ApplicantViewModel
            {
                Profile = applicant,
                User = user
            };

            return View("~/Views/Applicant/Profile/Edit.cshtml", viewModel);
        }

        /*
         * Route: /profile/data
         * Validates and returns the applicant’s profile data.
         */
        [HttpGet("profile/data")]
        public IActionResult GetApplicantProfile()
        {
            var email = Request.Cookies["userEmail"];
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var user = _userService.GetUserByEmail(email);
            var applicant = _applicantService.GetApplicantProfile(user.Id);
            if (applicant == null)
            {
                return BadRequest(new { success = false, message = "Invalid applicant profile data." });
            }

            return Ok(new { success = true });
        }

        /*
         * Route: /profile/create
         * Creates a new applicant profile with optional resume upload.
         */
        [HttpPost("profile/create")]
        public async Task<IActionResult> CreateApplicant([FromForm] ApplicantProfileCommand applicantProfileCommand, [FromForm] IFormFile resume)
        {
            if (resume != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await resume.CopyToAsync(memoryStream);
                    applicantProfileCommand.Resume = memoryStream.ToArray();
                }
            }

            var email = Request.Cookies["userEmail"];
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var user = _userService.GetUserByEmail(email);

            try
            {
                var applicantId = await _applicantService.SaveApplicantAsync(applicantProfileCommand, user, resume);
                return Ok(new { applicantId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /*
         * Route: /profile/edit/{id}
         * Edits an existing applicant profile and updates the resume if provided.
         */
        [HttpPut("profile/edit/{id}")]
        public async Task<IActionResult> EditApplicant([FromForm] ApplicantProfileCommand applicantProfileCommand, [FromForm] IFormFile resume, int id)
        {
            var applicant = _applicantService.GetApplicantProfile(id);
            if (applicant == null)
            {
                return NotFound("Applicant not found");
            }

            if (resume != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await resume.CopyToAsync(memoryStream);
                    applicantProfileCommand.Resume = memoryStream.ToArray();
                }
            }

            var email = Request.Cookies["userEmail"];
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var user = _userService.GetUserByEmail(email);

            try
            {
                var updatedApplicant = await _applicantService.UpdateApplicantAsync(id, applicantProfileCommand, user, resume);
                return Ok(new { updatedApplicant });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /*
         * Route: /apply
         * Allows an applicant to apply for a job after ensuring their profile is complete.
         */
        [HttpPost("apply")]
        public IActionResult Apply([FromBody] JobApplicationCommand jobApplicationCommand)
        {
            var email = Request.Cookies["userEmail"];
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var user = _userService.GetUserByEmail(email);
            var applicant = _applicantService.GetApplicantProfile(user.Id);
            if (applicant == null)
            {
                return BadRequest(new { success = false, message = "Please fill profile first." });
            }

            jobApplicationCommand.ApplicantId = user.Id;
            var applied = _applicantService.ApplyForJob(jobApplicationCommand);

            return Ok(new { success = true, data = applied });
        }

        /*
         * Route: /applied
         * Displays a paginated list of jobs the applicant has applied for.
         */
        [AllowAnonymous]
        [HttpGet("applied")]
        public IActionResult AppliedJobs(string title, string location, int pageNumber = 1, int pageSize = 10)
        {
            var email = Request.Cookies["userEmail"];
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var user = _userService.GetUserByEmail(email);
            var paginatedJobs = _applicantService.GetJobAppliedList(title, location, user.Id, pageNumber, pageSize);

            var viewModel = new JobApplicationViewModel
            {
                Jobs = paginatedJobs.Items,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling((double)paginatedJobs.TotalCount / pageSize),
                TotalItems = paginatedJobs.TotalCount,
                ItemsPerPage = pageSize,
                Message = !paginatedJobs.Items.Any() ? "No jobs available at the moment." : null
            };

            return View("~/Views/Applicant/AppliedJobs/Index.cshtml", viewModel);
        }

    }

}

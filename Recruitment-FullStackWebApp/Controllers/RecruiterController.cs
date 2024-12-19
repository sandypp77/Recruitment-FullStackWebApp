using Microsoft.AspNetCore.Mvc;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Services;

namespace Recruitment_FullStackWebApp.Controllers
{
    [Route("recruiter")]
    [Authorize]
    public class RecruiterController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRecruiterService _recruiterService;

        public RecruiterController(IRecruiterService recruiterService, IUserService userService)
        {
            _userService = userService;
            _recruiterService = recruiterService;
        }

        /*
         * Route: /
         * Displays job listings for a recruiter.
         */
        [AllowAnonymous]
        public IActionResult Index(string email, string title, string location)
        {
            if (string.IsNullOrEmpty(email))
            {
                email = Request.Cookies["userEmail"];
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email is required.");
                }
            }


            var user = _userService.GetUserByEmail(email);
            var jobs = _recruiterService.GetJobsByRecruiter(user.Id, title, location);

            var viewModel = new JobViewModel
            {
                Jobs = jobs,
                Message = jobs == null || !jobs.Any() ? "No jobs found for this recruiter." : null
            };
            return View("~/Views/Recruiter/JobListing/Index.cshtml", viewModel);
        }

        /*
         * Route: /create
         * Displays the job creation form.
         */
        [AllowAnonymous]
        [HttpGet("create")]
        public IActionResult CreateView()
        {
            return View("~/Views/Recruiter/JobListing/Create.cshtml");
        }

        /*
         * Route: /edit
         * Displays the job edit form for a specific job.
         */
        [AllowAnonymous]
        [HttpGet("edit")]
        public IActionResult EditView([FromQuery] int jobId)
        {
            var job = _recruiterService.GetJobById(jobId);
            var viewModel = new JobEditViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                Salary = job.Salary,
                JobType = job.JobTypeId
            };

            return View("~/Views/Recruiter/JobListing/Edit.cshtml", viewModel);
        }

        /*
         * Route: /all
         * Returns all jobs for the recruiter based on email.
         */
        [HttpGet("/all")]
        public IActionResult GetJobsByRecruiter()
        {
            var email = "";
            if (string.IsNullOrEmpty(email))
            {
                email = Request.Cookies["userEmail"];
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email is required.");
                }
            }


            var user = _userService.GetUserByEmail(email);

            var jobs = _recruiterService.GetJobsByRecruiter(user.Id, "", "");
            if (jobs == null || !jobs.Any())
            {
                return NotFound(new { message = "No jobs found for this recruiter." });
            }

            return Ok(jobs);
        }

        /*
         * Route: / (POST)
         * Creates a new job posting.
         */
        [HttpPost]
        public IActionResult CreateJob([FromForm] JobCommand jobCommand, [FromQuery] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid job data.", errors = ModelState });
            }

            var user = _userService.GetUserByEmail(email);
            var createdJob = _recruiterService.CreateJob(jobCommand, user.Id);

            if (createdJob == null)
            {
                return BadRequest(new { success = false, message = "Job creation failed." });
            }

            return Ok(new { success = true, message = "Job created successfully.", job = createdJob });
        }

        /*
         * Route: /{id}
         * Returns details of a job by its ID.
         */
        [HttpGet("{id}")]
        public IActionResult GetJobById(int id)
        {
            var job = _recruiterService.GetJobById(id);

            if (job == null)
            {
                return NotFound(new { message = "Job not found." });
            }

            return Ok(job);
        }

        /*
         * Route: /{id} (PUT)
         * Updates a specific job.
         */
        [HttpPut("{id}")]
        public IActionResult UpdateJob(int id, [FromQuery] string email, [FromForm] JobCommand jobCommand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid job data.", errors = ModelState });
            }

            var user = _userService.GetUserByEmail(email);
            var updatedJob = _recruiterService.UpdateJob(id, jobCommand, user.Id);

            if (updatedJob == null)
            {
                return Unauthorized(new { success = false, message = "You are not authorized to update this job or job does not exist." });
            }

            return Ok(new { success = true, message = "Job updated successfully.", job = updatedJob });
        }

        /*
         * Route: /{id} (DELETE)
         * Deletes a job.
         */
        [HttpDelete("{id}")]
        public IActionResult DeleteJob(int id, [FromQuery] string email)
        {
            var user = _userService.GetUserByEmail(email);
            var success = _recruiterService.DeleteJob(id, user.Id);

            if (!success)
            {
                return Unauthorized(new { success = false, message = "You are not authorized to delete this job or job does not exist." });
            }

            return Ok(new { success = true, message = "Job deleted successfully." });
        }

        /*
         * Route: /jobtype
         * Displays all job types.
         */
        [AllowAnonymous]
        [HttpGet("jobtype")]
        public IActionResult JobTypeIndex(string email)
        {
            var jobTypes = _recruiterService.GetAllJobTypes();

            var viewModel = new JobTypeViewModel
            {
                JobTypes = jobTypes,
                Message = jobTypes == null || !jobTypes.Any() ? "No job types found." : null
            };
            return View("~/Views/Recruiter/JobType/Index.cshtml", viewModel);
        }

        /*
         * Route: /jobtype/create
         * Displays the job type creation form.
         */
        [AllowAnonymous]
        [HttpGet("jobtype/create")]
        public IActionResult CreateJobTypeView()
        {
            return View("~/Views/Recruiter/JobType/Create.cshtml");
        }

        /*
         * Route: /jobtype/edit
         * Displays the job type edit form for a specific job type.
         */
        [AllowAnonymous]
        [HttpGet("jobtype/edit")]
        public IActionResult EditJobTypeView([FromQuery] int jobTypeId)
        {
            var job = _recruiterService.GetJobTypeById(jobTypeId);
            var viewModel = new JobTypeEditViewModel
            {
                Id = job.Id,
                Name = job.Name,
                Description = job.Description,
            };

            return View("~/Views/Recruiter/JobType/Edit.cshtml", viewModel);
        }

        /*
         * Route: /jobtype/all
         * Returns all job types.
         */
        [HttpGet("jobtype/all")]
        public IActionResult GetAllJobTypes()
        {
            var jobTypeList = _recruiterService.GetAllJobTypes();
            if (jobTypeList == null || !jobTypeList.Any())
            {
                return NotFound(new { message = "No job types found." });
            }

            return Ok(new { success = true, jobTypes = jobTypeList });
        }

        /*
         * Route: /jobtype (POST)
         * Creates a new job type.
         */
        [HttpPost("jobtype")]
        public IActionResult CreateJobType([FromForm] JobTypeCommand jobTypeCommand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid job data.", errors = ModelState });
            }

            var createdJobType = _recruiterService.CreateJobType(jobTypeCommand);

            if (createdJobType == null)
            {
                return BadRequest(new { success = false, message = "Job Type creation failed." });
            }

            return Ok(new { success = true, message = "Job Type created successfully.", job = createdJobType });
        }

        /*
         * Route: /jobtype/{id}
         * Returns job type details by ID.
         */
        [HttpGet("jobtype/{id}")]
        public IActionResult GetJobTypeById(int id)
        {
            var jobType = _recruiterService.GetJobTypeById(id);

            if (jobType == null)
            {
                return NotFound(new { message = "Job type not found." });
            }

            return Ok(jobType);
        }

        /*
         * Route: /jobtype/{id} (PUT)
         * Updates a job type.
         */
        [HttpPut("jobtype/{id}")]
        public IActionResult UpdateJobType(int id, [FromForm] JobTypeCommand jobTypeCommand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid job type data.", errors = ModelState });
            }

            var updatedJobType = _recruiterService.UpdateJobType(id, jobTypeCommand);

            if (updatedJobType == null)
            {
                return Unauthorized(new { success = false, message = "You are not authorized to update this job type or job type does not exist." });
            }

            return Ok(new { success = true, message = "Job type updated successfully.", job = updatedJobType });
        }

        /*
         * Route: /jobtype/{id} (DELETE)
         * Deletes a job type.
         */
        [HttpDelete("jobtype/{id}")]
        public IActionResult DeleteJobType(int id)
        {
            var success = _recruiterService.DeleteJobType(id);

            if (!success)
            {
                return Unauthorized(new { success = false, message = "You are not authorized to delete this job type or job type does not exist." });
            }

            return Ok(new { success = true, message = "Job type deleted successfully." });
        }

        /*
         * Route: /job/applicants/{jobId}
         * Displays a paginated list of applicants for a specific job.
         */
        [AllowAnonymous]
        [HttpGet("job/applicants/{jobId}")]
        public IActionResult JobWithApplicantsIndex(int jobId, int pageNumber = 1, int pageSize = 10)
        {

            var email = "";
            email = Request.Cookies["userEmail"];
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var user = _userService.GetUserByEmail(email);
            var job = _recruiterService.GetJobById(jobId);
            var paginatedApplicants = _recruiterService.GetJobApplicantList(jobId, pageNumber, pageSize);

            var viewModel = new ApplicantsAppliedViewModel
            {
                Applicants = paginatedApplicants.Items,
                Job = job,
                Recruiter = user,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling((double)paginatedApplicants.TotalCount / pageSize),
                TotalItems = paginatedApplicants.TotalCount,
                ItemsPerPage = pageSize,
                Message = !paginatedApplicants.Items.Any() ? "No applicants available at the moment." : null
            };


            return View("~/Views/Recruiter/ApplicantList/Index.cshtml", viewModel);
        }

        /*
         * Route: /job/applicants/{id} (PUT)
         * Updates the status of a job application.
         */
        [HttpPut("job/applicants/{id}")]
        public IActionResult UpdateStatus(int id, [FromBody] JobApplicationCommand jobApplicationCommand)
        {
            var updatedJobApplication = _recruiterService.UpdateJobApplication(id, jobApplicationCommand);

            if (updatedJobApplication == null)
            {
                return Unauthorized(new { success = false, message = "You are not authorized to update this job applicaton or job applicaton does not exist." });
            }

            return Ok(new { success = true, message = "Job applicaton updated successfully.", data = updatedJobApplication });

        }

    }

}

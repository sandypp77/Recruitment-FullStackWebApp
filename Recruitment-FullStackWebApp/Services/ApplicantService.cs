using AutoMapper;
using Recruitment_FullStackWebApp.Models;
using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Repositories;
using System.Numerics;

namespace Recruitment_FullStackWebApp.Services
{
    /// <summary>
    /// Provides services related to applicants, including managing their profiles and job applications.
    /// </summary>
    public class ApplicantService : IApplicantService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IApplicantRepository _applicantRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicantService"/> class.
        /// </summary>
        /// <param name="userRepository">The repository for managing users.</param>
        /// <param name="jobRepository">The repository for managing jobs.</param>
        /// <param name="applicantRepository">The repository for managing applicants.</param>
        /// <param name="mapper">The mapper for converting entities to DTOs.</param>
        /// <param name="environment">The web hosting environment.</param>
        public ApplicantService(IUserRepository userRepository, IJobRepository jobRepository, IApplicantRepository applicantRepository, IMapper mapper, IWebHostEnvironment environment)
        {
            _userRepository = userRepository;
            _jobRepository = jobRepository;
            _applicantRepository = applicantRepository;
            _mapper = mapper;
            _environment = environment;
        }

        /// <summary>
        /// Retrieves a paginated list of jobs based on the specified title, location, and pagination parameters.
        /// </summary>
        /// <param name="title">The job title to filter by.</param>
        /// <param name="location">The job location to filter by.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of jobs per page.</param>
        /// <returns>A <see cref="PaginationJobDto"/> containing the paginated list of jobs.</returns>
        public PaginationJobDto GetAllJobs(string title, string location, int pageNumber, int pageSize)
        {
            return _jobRepository.GetAllJobsPagination(title, location, pageNumber, pageSize);
        }

        /// <summary>
        /// Retrieves the profile of an applicant by their applicant ID.
        /// </summary>
        /// <param name="applicantId">The unique identifier of the applicant.</param>
        /// <returns>An <see cref="ApplicantProfileDto"/> containing the applicant's profile details.</returns>
        public ApplicantProfileDto GetApplicantProfile(int applicantId)
        {
            return _mapper.Map<ApplicantProfileDto>(_applicantRepository.GetApplicantProfile(applicantId));
        }

        /// <summary>
        /// Saves the applicant's profile asynchronously, including handling file upload for the resume.
        /// </summary>
        /// <param name="applicantProfileCommand">The command containing the applicant's profile details.</param>
        /// <param name="user">The user performing the action.</param>
        /// <param name="resumeFile">The resume file to be uploaded.</param>
        /// <returns>The unique identifier of the created applicant profile.</returns>
        public async Task<int> SaveApplicantAsync(ApplicantProfileCommand applicantProfileCommand, UserDto user, IFormFile resumeFile)
        {
            ApplicantProfileDto applicantProfileDto = _mapper.Map<ApplicantProfileDto>(applicantProfileCommand);
            if (resumeFile != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(resumeFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await resumeFile.CopyToAsync(fileStream);
                }

                applicantProfileDto.ResumeUrl = $"/uploads/{uniqueFileName}";
            }

            return await _applicantRepository.CreateProfile(new Applicant()
            {
                FullName = applicantProfileDto.FullName,
                Phone = applicantProfileDto.Phone,
                Address = applicantProfileDto.Address,
                ResumeUrl = applicantProfileDto.ResumeUrl,
                Skills = applicantProfileDto.Skills,
                DateOfBirth = applicantProfileDto.DateOfBirth,
                ApplicantId = user.Id,
            });
        }

        /// <summary>
        /// Updates an existing applicant's profile asynchronously, including handling file upload for the resume.
        /// </summary>
        /// <param name="applicantId">The unique identifier of the applicant.</param>
        /// <param name="applicantProfileCommand">The command containing the updated applicant's profile details.</param>
        /// <param name="user">The user performing the action.</param>
        /// <param name="resumeFile">The resume file to be uploaded.</param>
        /// <returns>An updated <see cref="ApplicantProfileDto"/> representing the applicant's profile.</returns>
        public async Task<ApplicantProfileDto> UpdateApplicantAsync(int applicantId, ApplicantProfileCommand applicantProfileCommand, UserDto user, IFormFile resumeFile)
        {
            var applicant = _applicantRepository.GetApplicantProfile(applicantId);

            applicant.FullName = applicantProfileCommand.FullName;
            applicant.Phone = applicantProfileCommand.Phone;
            applicant.Address = applicantProfileCommand.Address;
            applicant.Skills = applicantProfileCommand.Skills;
            applicant.DateOfBirth = applicantProfileCommand.DateOfBirth;

            if (resumeFile != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(resumeFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await resumeFile.CopyToAsync(fileStream);
                }

                applicant.ResumeUrl = $"/uploads/{uniqueFileName}";
            }

            await _applicantRepository.UpdateProfile(applicant);
            return _mapper.Map<ApplicantProfileDto>(applicant);
        }

        /// <summary>
        /// Applies for a job on behalf of an applicant.
        /// </summary>
        /// <param name="jobApplicationCommand">The command containing the job application details.</param>
        /// <returns>A <see cref="JobApplicationDto"/> representing the job application.</returns>
        public JobApplicationDto ApplyForJob(JobApplicationCommand jobApplicationCommand)
        {
            var jobApplicantDto = _mapper.Map<JobApplicationDto>(jobApplicationCommand);

            _jobRepository.ApplyForJob(new JobApplication
            {
                JobId = jobApplicantDto.JobId,
                ApplicantId = jobApplicantDto.ApplicantId,
                ApplicationDate = jobApplicantDto.ApplicationDate,
                Status = jobApplicantDto.Status,
            });

            return jobApplicantDto;
        }

        /// <summary>
        /// Retrieves a paginated list of jobs applied by a specific applicant.
        /// </summary>
        /// <param name="title">The job title to filter by.</param>
        /// <param name="location">The job location to filter by.</param>
        /// <param name="applicantId">The unique identifier of the applicant.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of jobs per page.</param>
        /// <returns>A <see cref="PaginationJobAppliedDto"/> containing the paginated list of jobs applied by the applicant.</returns>
        public PaginationJobAppliedDto GetJobAppliedList(string title, string location, int applicantId, int pageNumber, int pageSize)
        {
            return _jobRepository.GetJobsAppliedByApplicant(title, location, applicantId, pageNumber, pageSize);
        }
    }
}

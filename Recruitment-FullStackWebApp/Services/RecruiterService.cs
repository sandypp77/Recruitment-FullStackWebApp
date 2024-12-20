using AutoMapper;
using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Models;
using Recruitment_FullStackWebApp.Repositories;

namespace Recruitment_FullStackWebApp.Services
{
    /// <summary>
    /// Provides services for managing job postings, applications, and job types by recruiters.
    /// </summary>
    public class RecruiterService : IRecruiterService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecruiterService"/> class.
        /// </summary>
        /// <param name="jobRepository">The repository for managing job postings and job applications.</param>
        /// <param name="mapper">The object mapper to convert entities to DTOs.</param>
        public RecruiterService(IJobRepository jobRepository, IMapper mapper)
        {
            _jobRepository = jobRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of jobs posted by a recruiter, optionally filtered by title and location.
        /// </summary>
        /// <param name="recruiterId">The unique identifier of the recruiter.</param>
        /// <param name="title">The job title to filter by (optional).</param>
        /// <param name="location">The job location to filter by (optional).</param>
        /// <returns>A list of <see cref="JobDto"/> containing job details.</returns>
        public IList<JobDto> GetJobsByRecruiter(int recruiterId, string title, string location)
        {
            var jobs = _jobRepository.GetJobsByRecruiter(recruiterId, title, location);
            return _mapper.Map<IList<JobDto>>(jobs);
        }

        /// <summary>
        /// Creates a new job posting by a recruiter.
        /// </summary>
        /// <param name="jobCommand">The details of the job to be created.</param>
        /// <param name="recruiterId">The unique identifier of the recruiter posting the job.</param>
        /// <returns>A <see cref="JobDto"/> containing the details of the created job.</returns>
        public JobDto CreateJob(JobCommand jobCommand, int recruiterId)
        {
            var jobType = _jobRepository.GetJobTypeById(jobCommand.JobType);
            var job = new Job()
            {
                Title = jobCommand.Title,
                Location = jobCommand.Location,
                Description = jobCommand.Description,
                Salary = jobCommand.Salary,
                RecruiterId = recruiterId,
                JobTypeId = jobType.Id,
            };

            var jobId = _jobRepository.Add(job);
            job.Id = jobId;

            return _mapper.Map<JobDto>(job);
        }

        /// <summary>
        /// Updates an existing job posting.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job to be updated.</param>
        /// <param name="jobCommand">The updated job details.</param>
        /// <param name="recruiterId">The unique identifier of the recruiter posting the job.</param>
        /// <returns>A <see cref="JobDto"/> containing the updated job details, or <c>null</c> if the job doesn't exist or belongs to another recruiter.</returns>
        public JobDto UpdateJob(int jobId, JobCommand jobCommand, int recruiterId)
        {
            var existingJob = _jobRepository.GetById(jobId);
            if (existingJob == null || existingJob.RecruiterId != recruiterId)
            {
                return null;
            }
            var jobType = _jobRepository.GetJobTypeById(jobCommand.JobType);

            existingJob.Title = jobCommand.Title;
            existingJob.Location = jobCommand.Location;
            existingJob.Description = jobCommand.Description;
            existingJob.Salary = jobCommand.Salary;
            existingJob.RecruiterId = recruiterId;
            existingJob.JobTypeId = jobType.Id;

            _jobRepository.Update(existingJob);
            return _mapper.Map<JobDto>(existingJob);
        }

        /// <summary>
        /// Deletes a job posting.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job to be deleted.</param>
        /// <param name="recruiterId">The unique identifier of the recruiter posting the job.</param>
        /// <returns><c>true</c> if the job was successfully deleted, <c>false</c> if the job doesn't exist or belongs to another recruiter.</returns>
        public bool DeleteJob(int jobId, int recruiterId)
        {
            var existingJob = _jobRepository.GetById(jobId);
            if (existingJob == null || existingJob.RecruiterId != recruiterId)
            {
                return false;
            }

            _jobRepository.Delete(jobId);
            return true;
        }

        /// <summary>
        /// Retrieves the details of a specific job posting by its ID.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <returns>A <see cref="JobDto"/> containing the job details.</returns>
        public JobDto GetJobById(int jobId)
        {
            return _mapper.Map<JobDto>(_jobRepository.GetById(jobId));
        }

        /// <summary>
        /// Retrieves a list of all available job types.
        /// </summary>
        /// <returns>A list of <see cref="JobTypeDto"/> representing available job types.</returns>
        public IList<JobTypeDto> GetAllJobTypes()
        {
            return _mapper.Map<IList<JobTypeDto>>(_jobRepository.GetAllJobTypes());
        }

        /// <summary>
        /// Retrieves a specific job type by its ID.
        /// </summary>
        /// <param name="jobTypeId">The unique identifier of the job type.</param>
        /// <returns>A <see cref="JobTypeDto"/> representing the job type.</returns>
        public JobTypeDto GetJobTypeById(int jobTypeId)
        {
            return _mapper.Map<JobTypeDto>(_jobRepository.GetJobTypeById(jobTypeId));
        }

        /// <summary>
        /// Creates a new job type.
        /// </summary>
        /// <param name="jobTypeCommand">The details of the job type to be created.</param>
        /// <returns>A <see cref="JobTypeDto"/> representing the created job type.</returns>
        public JobTypeDto CreateJobType(JobTypeCommand jobTypeCommand)
        {
            var jobType = _mapper.Map<JobType>(jobTypeCommand);

            var jobTypeId = _jobRepository.AddJobType(jobType);
            jobType.Id = jobTypeId;

            return _mapper.Map<JobTypeDto>(jobType);
        }

        /// <summary>
        /// Updates an existing job type.
        /// </summary>
        /// <param name="jobTypeId">The unique identifier of the job type to be updated.</param>
        /// <param name="jobTypeCommand">The updated job type details.</param>
        /// <returns>A <see cref="JobTypeDto"/> representing the updated job type, or <c>null</c> if the job type doesn't exist.</returns>
        public JobTypeDto UpdateJobType(int jobTypeId, JobTypeCommand jobTypeCommand)
        {
            var existingJob = _jobRepository.GetJobTypeById(jobTypeId);
            if (existingJob == null)
            {
                return null;
            }

            var jobType = _mapper.Map<JobType>(jobTypeCommand);

            _jobRepository.UpdateJobType(jobType);
            return _mapper.Map<JobTypeDto>(jobType);
        }

        /// <summary>
        /// Deletes a job type.
        /// </summary>
        /// <param name="jobTypeId">The unique identifier of the job type to be deleted.</param>
        /// <returns><c>true</c> if the job type was successfully deleted, <c>false</c> if the job type doesn't exist.</returns>
        public bool DeleteJobType(int jobTypeId)
        {
            var existingJob = _jobRepository.GetJobTypeById(jobTypeId);
            if (existingJob == null)
            {
                return false;
            }

            _jobRepository.Delete(jobTypeId);
            return true;
        }

        /// <summary>
        /// Retrieves a list of applicants for a specific job, with pagination support.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The page size for pagination.</param>
        /// <returns>A <see cref="PaginationJobAppliedDto"/> containing a list of applicants for the job.</returns>
        public PaginationJobAppliedDto GetJobApplicantList(int jobId, int pageNumber, int pageSize)
        {
            return _jobRepository.GetJobApplicants(jobId, pageNumber, pageSize);
        }

        /// <summary>
        /// Updates the status of a job application.
        /// </summary>
        /// <param name="jobApplicationId">The unique identifier of the job application to be updated.</param>
        /// <param name="jobApplicationCommand">The new status for the job application.</param>
        /// <returns>A <see cref="JobApplicationDto"/> containing the updated application status, or <c>null</c> if the application doesn't exist.</returns>
        public JobApplicationDto UpdateJobApplication(int jobApplicationId, JobApplicationCommand jobApplicationCommand)
        {
            var existingJobApplication = _jobRepository.GetJobApplicationById(jobApplicationId);
            if (existingJobApplication == null)
            {
                return null;
            }

            existingJobApplication.Status = jobApplicationCommand.Status;

            _jobRepository.UpdateJobApplication(existingJobApplication);
            return _mapper.Map<JobApplicationDto>(existingJobApplication);
        }
    }
}

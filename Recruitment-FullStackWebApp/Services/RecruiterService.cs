using AutoMapper;
using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Models;
using Recruitment_FullStackWebApp.Repositories;

namespace Recruitment_FullStackWebApp.Services
{
    public class RecruiterService : IRecruiterService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;

        public RecruiterService(IJobRepository jobRepository, IMapper mapper)
        {
            _jobRepository = jobRepository;
            _mapper = mapper;
        }

        public IList<JobDto> GetJobsByRecruiter(int recruiterId, string title, string location)
        {
            var jobs = _jobRepository.GetJobsByRecruiter(recruiterId, title, location);
            return _mapper.Map<IList<JobDto>>(jobs);
        }

        public JobDto CreateJob(JobCommand jobCommand, int recruiterId)
        {
            var job = _mapper.Map<Job>(jobCommand);
            job.RecruiterId = recruiterId;

            var jobId = _jobRepository.Add(job);
            job.Id = jobId;

            return _mapper.Map<JobDto>(job);
        }

        public JobDto UpdateJob(int jobId, JobCommand jobCommand, int recruiterId)
        {
            var existingJob = _jobRepository.GetById(jobId);
            if (existingJob == null || existingJob.RecruiterId != recruiterId)
            {
                return null;
            }

            var job = _mapper.Map(jobCommand, existingJob);

            _jobRepository.Update(job);
            return _mapper.Map<JobDto>(job);
        }

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

        public JobDto GetJobById(int jobId)
        {
            return _mapper.Map<JobDto>(_jobRepository.GetById(jobId));
        }

        public IList<JobTypeDto> GetAllJobTypes()
        {
            return _mapper.Map<IList<JobTypeDto>>(_jobRepository.GetAllJobTypes());
        }

        public JobTypeDto GetJobTypeById(int jobTypeId)
        {
            return _mapper.Map<JobTypeDto>(_jobRepository.GetJobTypeById(jobTypeId));
        }

        public JobTypeDto CreateJobType(JobTypeCommand jobTypeCommand)
        {
            var jobType = _mapper.Map<JobType>(jobTypeCommand);

            var jobTypeId = _jobRepository.AddJobType(jobType);
            jobType.Id = jobTypeId;

            return _mapper.Map<JobTypeDto>(jobType);
        }

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

        public PaginationJobAppliedDto GetJobApplicantList(int jobId, int pageNumber, int pageSize)
        {
            return _jobRepository.GetJobApplicants(jobId, pageNumber, pageSize);
        }

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

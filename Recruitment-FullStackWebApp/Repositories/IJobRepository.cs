using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Models;

namespace Recruitment_FullStackWebApp.Repositories
{
    public interface IJobRepository
    {
        IList<Job> GetJobsByRecruiter(int recruiterId, string title, string location);
        IList<Job> GetAllJobs();
        PaginationJobDto GetAllJobsPagination(string title, string location, int pageNumber, int pageSize);
        Job GetById(int jobId);
        IList<JobType> GetAllJobTypes();
        JobType GetJobTypeById(int jobTypeId);
        int Add(Job job);
        void Update(Job job);
        void Delete(int jobId);
        int AddJobType(JobType jobType);
        void UpdateJobType(JobType jobType);
        void DeleteJobType(int jobTypeId);
        int ApplyForJob(JobApplication jobApplication);
        void UpdateJobApplication(JobApplication jobApplication);
        JobApplication GetJobApplicationById(int jobApplicationId);
        IList<JobApplication> GetApplicationsByApplicant(int applicantId);
        PaginationJobAppliedDto GetJobsAppliedByApplicant(string title, string location, int applicantId, int pageNumber, int pageSize);
        PaginationJobAppliedDto GetJobApplicants(int jobId, int pageNumber, int pageSize);
    }
}

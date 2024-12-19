using Recruitment_FullStackWebApp.Models;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Common.Commands;

namespace Recruitment_FullStackWebApp.Services
{
    public interface IRecruiterService
    {
        IList<JobDto> GetJobsByRecruiter(int recruiterId, string title, string location);
        JobDto GetJobById(int jobId);
        JobDto CreateJob(JobCommand jobCommand, int recruiterId);
        JobDto UpdateJob(int jobId, JobCommand jobCommand, int recruiterId);
        bool DeleteJob(int jobId, int recruiterId);
        IList<JobTypeDto> GetAllJobTypes();
        JobTypeDto GetJobTypeById(int jobTypeId);
        JobTypeDto CreateJobType(JobTypeCommand jobTypeCommand);
        JobTypeDto UpdateJobType(int jobTypeId, JobTypeCommand jobTypeCommand);
        bool DeleteJobType(int jobTypeId);
        PaginationJobAppliedDto GetJobApplicantList(int jobId, int pageNumber, int pageSize);
        JobApplicationDto UpdateJobApplication(int jobApplicationId, JobApplicationCommand jobApplicationCommand);
    }
}

using Recruitment_FullStackWebApp.Models;
using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Common.Dtos;

namespace Recruitment_FullStackWebApp.Services
{
    public interface IApplicantService
    {
        PaginationJobDto GetAllJobs(string title, string location, int pageNumber, int pageSize);
        Task<int> SaveApplicantAsync(ApplicantProfileCommand applicantProfileCommand, UserDto user, IFormFile resumeFile);
        Task<ApplicantProfileDto> UpdateApplicantAsync(int applicantId, ApplicantProfileCommand applicantProfileCommand, UserDto user, IFormFile resumeFile);
        ApplicantProfileDto GetApplicantProfile(int applicantId);
        JobApplicationDto ApplyForJob(JobApplicationCommand jobApplicationCommand);
        PaginationJobAppliedDto GetJobAppliedList(string title, string location, int applicantId, int pageNumber, int pageSize);
    }
}

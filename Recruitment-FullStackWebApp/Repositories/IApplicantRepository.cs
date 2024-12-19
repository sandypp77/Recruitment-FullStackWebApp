using Recruitment_FullStackWebApp.Models;

namespace Recruitment_FullStackWebApp.Repositories
{
    public interface IApplicantRepository
    {
        Task<int> CreateProfile(Applicant applicant);
        Task<Applicant> UpdateProfile(Applicant applicant);
        Applicant GetApplicantProfile(int applicantId);
    }
}

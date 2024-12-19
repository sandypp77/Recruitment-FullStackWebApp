using Recruitment_FullStackWebApp.Common.Dtos;

namespace Recruitment_FullStackWebApp.Services
{
    public interface IFileService
    {
        FileDataDto GetResumeFile(int applicantId);
    }

}

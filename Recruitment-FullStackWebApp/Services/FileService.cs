using Microsoft.AspNetCore.StaticFiles;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Repositories;

namespace Recruitment_FullStackWebApp.Services
{
    public class FileService : IFileService
    {
        private readonly IApplicantRepository _applicantRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IWebHostEnvironment _environment;

        public FileService(IApplicantRepository applicantRepository, IJobRepository jobRepository, IWebHostEnvironment environment)
        {
            _applicantRepository = applicantRepository;
            _jobRepository = jobRepository;
            _environment = environment;
        }

        public FileDataDto GetResumeFile(int applicantId)
        {
            var applicant = _applicantRepository.GetApplicantProfile(applicantId);
            if (applicant == null || string.IsNullOrEmpty(applicant.ResumeUrl))
                throw new FileNotFoundException("Resume not found.");
            var resumePath = applicant.ResumeUrl.TrimStart('/');
            resumePath = resumePath.Replace('/', '\\');
            var filePath = Path.Combine(_environment.WebRootPath, resumePath);


            if (!File.Exists(filePath))
                throw new FileNotFoundException("Resume file does not exist.");
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!contentTypeProvider.TryGetContentType(filePath, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return new FileDataDto
            {
                FileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read),
                ContentType = contentType,
                FileName = Path.GetFileName(filePath)
            };
        }
    }

}

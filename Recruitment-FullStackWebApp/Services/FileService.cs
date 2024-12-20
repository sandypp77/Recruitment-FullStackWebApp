using Microsoft.AspNetCore.StaticFiles;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Repositories;

namespace Recruitment_FullStackWebApp.Services
{
    /// <summary>
    /// Provides services for handling files, such as retrieving resume files for applicants.
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IApplicantRepository _applicantRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileService"/> class.
        /// </summary>
        /// <param name="applicantRepository">The repository for managing applicants.</param>
        /// <param name="jobRepository">The repository for managing jobs.</param>
        /// <param name="environment">The web hosting environment.</param>
        public FileService(IApplicantRepository applicantRepository, IJobRepository jobRepository, IWebHostEnvironment environment)
        {
            _applicantRepository = applicantRepository;
            _jobRepository = jobRepository;
            _environment = environment;
        }

        /// <summary>
        /// Retrieves the resume file of an applicant.
        /// </summary>
        /// <param name="applicantId">The unique identifier of the applicant.</param>
        /// <returns>A <see cref="FileDataDto"/> containing the file stream, content type, and file name of the resume.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the resume file is not found.</exception>
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

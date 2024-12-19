using AutoMapper;
using Recruitment_FullStackWebApp.Models;
using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Repositories;
using System.Numerics;

namespace Recruitment_FullStackWebApp.Services
{
    public class ApplicantService : IApplicantService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IApplicantRepository _applicantRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public ApplicantService(IUserRepository userRepository, IJobRepository jobRepository, IApplicantRepository applicantRepository, IMapper mapper, IWebHostEnvironment environment)
        {
            _userRepository = userRepository;
            _jobRepository = jobRepository;
            _applicantRepository = applicantRepository;
            _mapper = mapper;
            _environment = environment;
        }

        public PaginationJobDto GetAllJobs(string title, string location, int pageNumber, int pageSize)
        {
            return _jobRepository.GetAllJobsPagination(title, location, pageNumber, pageSize);
        }

        public ApplicantProfileDto GetApplicantProfile(int applicantId)
        {
            return _mapper.Map<ApplicantProfileDto>(_applicantRepository.GetApplicantProfile(applicantId));
        }

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

        public PaginationJobAppliedDto GetJobAppliedList(string title, string location, int applicantId, int pageNumber, int pageSize)
        {
            return _jobRepository.GetJobsAppliedByApplicant(title, location, applicantId, pageNumber, pageSize);
        }
    }
}

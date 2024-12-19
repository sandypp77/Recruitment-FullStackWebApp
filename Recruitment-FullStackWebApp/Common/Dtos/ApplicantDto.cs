namespace Recruitment_FullStackWebApp.Common.Dtos
{
    public class PaginationJobDto
    {
        public IList<JobDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class ApplicantProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ResumeUrl { get; set; }
        public string Skills { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class ApplicantDataDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ResumeUrl { get; set; }
        public string Skills { get; set; }
        public DateTime DateOfBirth { get; set; }
        public UserDto User { get; set; }
        public JobApplicationDto JobApplication { get; set; }
    }

    public class PaginationJobAppliedDto
    {
        public IList<JobWithApplicantsDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

}

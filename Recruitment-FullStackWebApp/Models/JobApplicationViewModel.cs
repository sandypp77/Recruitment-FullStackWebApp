using Recruitment_FullStackWebApp.Common.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Recruitment_FullStackWebApp.Common.Dtos;

namespace Recruitment_FullStackWebApp.Models
{
    public class JobApplicationViewModel
    {
        public IList<JobWithApplicantsDto> Jobs { get; set; }
        public string Message { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    public class ApplicantsAppliedViewModel
    {
        public IList<JobWithApplicantsDto> Applicants { get; set; }
        public JobDto Job { get; set; }
        public UserDto Recruiter { get; set; }
        public string Message { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}

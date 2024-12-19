using Recruitment_FullStackWebApp.Common.Dtos;

namespace Recruitment_FullStackWebApp.Models
{
    public class JobViewModel
    {
        public IList<JobDto> Jobs { get; set; }
        public string Message { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    public class JobEditViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal Salary { get; set; }
        public int JobType { get; set; }
    }
}

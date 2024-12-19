using Recruitment_FullStackWebApp.Common.Enum;

namespace Recruitment_FullStackWebApp.Common.Commands
{
    public class JobCommand
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal Salary { get; set; }
        public string Category { get; set; }
    }

    public class JobTypeCommand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class JobApplicationCommand
    {
        public int JobId { get; set; }
        public int ApplicantId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public JobApplicationEnum Status { get; set; }
    }
}

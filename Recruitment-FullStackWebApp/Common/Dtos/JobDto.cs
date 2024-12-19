using Recruitment_FullStackWebApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Recruitment_FullStackWebApp.Common.Enum;

namespace Recruitment_FullStackWebApp.Common.Dtos
{
    public class JobDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal Salary { get; set; }
        public string Category { get; set; }
    }

    public class JobTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class JobApplicationDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int ApplicantId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public JobApplicationEnum Status { get; set; }
    }

    public class JobWithApplicantsDto
    {
        public int JobId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal Salary { get; set; }
        public string Category { get; set; }
        public int? JobApplicationId { get; set; }
        public int? ApplicantId { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public string Status { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantPhone { get; set; }
        public string ApplicantAddress { get; set; }
        public string ResumeUrl { get; set; }
        public string Skills { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

}

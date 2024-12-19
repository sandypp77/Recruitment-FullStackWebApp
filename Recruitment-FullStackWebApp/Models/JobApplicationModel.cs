using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Recruitment_FullStackWebApp.Common.Enum;

namespace Recruitment_FullStackWebApp.Models
{
    public class JobApplication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JobId { get; set; }

        [ForeignKey("JobId")]
        public Job Job { get; set; }

        [Required]
        public int ApplicantId { get; set; }

        [ForeignKey("ApplicantId")]
        public Applicant Applicant { get; set; }

        [Required]
        public DateTime ApplicationDate { get; set; }

        [Required]
        [MaxLength(50)]
        public JobApplicationEnum Status { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Recruitment_FullStackWebApp.Models
{
    public class Job
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Salary { get; set; }

        [Required]
        public int JobTypeId { get; set; }

        [ForeignKey("JobTypeId")]
        public JobType JobType { get; set; }

        [Required]
        public int RecruiterId { get; set; }

        [ForeignKey("RecruiterId")]
        public User Recruiter { get; set; }

    }
}

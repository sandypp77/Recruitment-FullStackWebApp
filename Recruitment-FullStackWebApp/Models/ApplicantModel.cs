using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recruitment_FullStackWebApp.Models
{
    public class Applicant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Phone { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address { get; set; }

        [Required]
        [MaxLength(500)]
        public string ResumeUrl { get; set; }

        [Required]
        [MaxLength(100)]
        public string Skills { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public int ApplicantId { get; set; }

        [ForeignKey("ApplicantId")]
        public User User { get; set; }
    }


}

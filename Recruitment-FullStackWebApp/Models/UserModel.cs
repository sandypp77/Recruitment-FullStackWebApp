using System.ComponentModel.DataAnnotations;

namespace Recruitment_FullStackWebApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public bool IsRecruiter { get; set; }
    }

}

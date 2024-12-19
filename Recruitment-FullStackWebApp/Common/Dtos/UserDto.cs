namespace Recruitment_FullStackWebApp.Common.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsRecruiter { get; set; }
    }
}

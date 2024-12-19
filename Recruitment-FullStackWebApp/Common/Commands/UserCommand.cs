namespace Recruitment_FullStackWebApp.Common.Commands
{
    public class UserCommand
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsRecruiter { get; set; }
    }
}

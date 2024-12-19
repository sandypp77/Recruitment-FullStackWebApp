namespace Recruitment_FullStackWebApp.Common.Commands
{
    public class ApplicantProfileCommand
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public byte[] Resume { get; set; }
        public string Skills { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}

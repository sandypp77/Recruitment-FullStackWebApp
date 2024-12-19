using Recruitment_FullStackWebApp.Common.Dtos;

namespace Recruitment_FullStackWebApp.Models
{
    public class JobTypeViewModel
    {
        public IList<JobTypeDto> JobTypes { get; set; }
        public string Message { get; set; }
    }

    public class JobTypeEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

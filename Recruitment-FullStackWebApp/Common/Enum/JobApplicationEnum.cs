using System.ComponentModel.DataAnnotations;

namespace Recruitment_FullStackWebApp.Common.Enum
{
    public enum JobApplicationEnum
    {
        [Display(Name = "In Review")]
        InReview,

        [Display(Name = "Rejected")]
        Rejected,

        [Display(Name = "Accepted")]
        Accepted
    }
}

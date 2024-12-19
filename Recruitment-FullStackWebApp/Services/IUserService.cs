using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Common.Dtos;

namespace Recruitment_FullStackWebApp.Services
{
    public interface IUserService
    {
        UserDto Authenticate(string email, string password);
        UserDto GetUserByEmail(string email);
        void Register(UserCommand userCommand);
        string GenerateJwtToken(UserDto user);
    }

}

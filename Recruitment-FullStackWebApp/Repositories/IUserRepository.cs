using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Common.Dtos;

namespace Recruitment_FullStackWebApp.Repositories
{
    public interface IUserRepository
    {
        UserDto GetUserByEmailAndPassword(string email, string password);
        UserDto GetUserByEmail(string email);
        void AddUser(UserCommand user);
    }
}

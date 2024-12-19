using Dapper;
using Microsoft.Data.SqlClient;
using AutoMapper;
using BCrypt.Net;
using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Models;

namespace Recruitment_FullStackWebApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;

        public UserRepository(string connectionString, IMapper mapper)
        {
            _connectionString = connectionString;
            _mapper = mapper;
        }

        public void AddUser(UserCommand user)
        {
            using var connection = new SqlConnection(_connectionString);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            const string sql = "INSERT INTO Users (Email, Password, IsRecruiter) VALUES (@Email, @Password, @IsRecruiter)";
            connection.Execute(sql, new { user.Email, Password = hashedPassword, user.IsRecruiter });
        }

        public UserDto GetUserByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Users WHERE Email = @Email";
            var user = connection.QuerySingleOrDefault<User>(sql, new { Email = email });
            return _mapper.Map<UserDto>(user);
        }

        public UserDto GetUserByEmailAndPassword(string email, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Users WHERE Email = @Email";
            var user = connection.QuerySingleOrDefault<User>(sql, new { Email = email });

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}

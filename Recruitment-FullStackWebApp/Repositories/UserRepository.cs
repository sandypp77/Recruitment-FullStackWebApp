using Dapper;
using Microsoft.Data.SqlClient;
using AutoMapper;
using BCrypt.Net;
using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Models;

namespace Recruitment_FullStackWebApp.Repositories
{
    /// <summary>
    /// Represents the repository for managing user data in the database.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="mapper">The mapper for converting entities to DTOs.</param>
        public UserRepository(string connectionString, IMapper mapper)
        {
            _connectionString = connectionString;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="user">The user command containing the user's details.</param>
        public void AddUser(UserCommand user)
        {
            using var connection = new SqlConnection(_connectionString);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            const string sql = "INSERT INTO Users (Email, Password, IsRecruiter) VALUES (@Email, @Password, @IsRecruiter)";
            connection.Execute(sql, new { user.Email, Password = hashedPassword, user.IsRecruiter });
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A <see cref="UserDto"/> representing the user's details, or null if not found.</returns>
        public UserDto GetUserByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Users WHERE Email = @Email";
            var user = connection.QuerySingleOrDefault<User>(sql, new { Email = email });
            return _mapper.Map<UserDto>(user);
        }

        /// <summary>
        /// Retrieves a user by their email and password.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <param name="password">The password of the user to verify.</param>
        /// <returns>A <see cref="UserDto"/> representing the user's details, or null if not found or password is incorrect.</returns>
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

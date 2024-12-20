using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Common.Dtos;
using Recruitment_FullStackWebApp.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Recruitment_FullStackWebApp.Services
{
    /// <summary>
    /// Provides services for managing user authentication, registration, and JWT token generation.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration settings for the application, including JWT keys and issuer details.</param>
        /// <param name="userRepository">The repository for managing user data.</param>
        /// <param name="mapper">The object mapper to convert user entities to DTOs.</param>
        public UserService(IConfiguration configuration, IUserRepository userRepository, IMapper mapper)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Authenticates a user by email and password.
        /// </summary>
        /// <param name="email">The email of the user attempting to log in.</param>
        /// <param name="password">The password provided by the user.</param>
        /// <returns>A <see cref="UserDto"/> object if authentication is successful, otherwise null.</returns>
        public UserDto Authenticate(string email, string password)
        {
            var user = _userRepository.GetUserByEmailAndPassword(email, password);
            if (user == null) return null;

            return _mapper.Map<UserDto>(user);
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>A <see cref="UserDto"/> object representing the user if found, otherwise null.</returns>
        public UserDto GetUserByEmail(string email)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null) return null;
            return _mapper.Map<UserDto>(user);
        }

        /// <summary>
        /// Generates a JWT token for the user, including claims such as email and recruiter status.
        /// </summary>
        /// <param name="user">The user for whom the JWT token is being generated.</param>
        /// <returns>A JWT token as a string.</returns>
        public string GenerateJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("IsRecruiter", user.IsRecruiter.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Registers a new user by adding them to the repository.
        /// </summary>
        /// <param name="userCommand">The user data used for registration.</param>
        public void Register(UserCommand userCommand)
        {
            _userRepository.AddUser(userCommand);
        }
    }
}

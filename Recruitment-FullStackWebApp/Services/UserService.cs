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
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IConfiguration configuration, IUserRepository userRepository, IMapper mapper)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public UserDto Authenticate(string email, string password)
        {
            var user = _userRepository.GetUserByEmailAndPassword(email, password);
            if (user == null) return null;

            return _mapper.Map<UserDto>(user);
        }

        public UserDto GetUserByEmail(string email)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null) return null;
            return _mapper.Map<UserDto>(user);
        }

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

        public void Register(UserCommand userCommand)
        {
            _userRepository.AddUser(userCommand);
        }
    }
}

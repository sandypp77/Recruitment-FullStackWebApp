using Recruitment_FullStackWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Recruitment_FullStackWebApp.Common.Dtos;
using System.Security.Claims;
using Recruitment_FullStackWebApp.Common.Commands;
using Recruitment_FullStackWebApp.Services;

namespace Recruitment_FullStackWebApp.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Users/Authentication.cshtml");
        }

        /*
        * Route: /login (POST)
        * Authenticates the user with email and password, generates a JWT token upon success.
        */
        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var user = _userService.Authenticate(email, password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = _userService.GenerateJwtToken(user);
            ViewBag.Token = token;

            return Ok(new { token, user });
        }

        /*
        * Route: /register (POST)
        * Registers a new user with the provided user details.
        */
        [HttpPost("/register")]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserCommand userCommand)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid input." });

            try
            {
                _userService.Register(userCommand);
                return Ok(new { success = true, message = "Registration successful!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Registration failed. Please try again later." });
            }

        }

    }

}

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VacationAPI.DTOs;
using VacationAPI.Services;
using VacationAPI.Helpers;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using VacationAPI.Extensions;

namespace VacationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;



        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpGet("checkauth")]
        [SwaggerOperation(Summary = "Check if user is authenticated successfully and returns username and role", Description = "Checks if the user is authenticated via a Bearer JWT token and returns the username and role if the user is authorized.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The user is authorized and the username and role are returned as a JSON object.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authenticated.")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAuthentication()
        {
            try
            {
                // Get the user ID from the ClaimsPrincipal
                var userId = User.GetUserId();

                // Use the user ID to get the user object
                var user = await _userService.GetById(userId);

                if (user == null)
                {
                    return NotFound(); // Return a 401
                }

                // Create a new anonymous object with the user's name and role
                var authenticatedUser = new
                {
                    user.UserName,
                    user.Role
                };

                return Ok(authenticatedUser); // Return a 200 OK response with the authenticatedUser object
            }
            catch (Exception)
            {
                return Unauthorized(); // Return a 401 Unauthorized response if there is an exception
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Login an existing user",
            Description = "Authenticates a user and returns a JWT token for authorization on API endpoints"
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a JWT token for the authenticated user")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid username or password")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var user = await _userService.GetByUsername(loginDTO.UserName);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid username or password" });
            }

            if (!PasswordHelper.VerifyPasswordHashAndSalt(loginDTO.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest(new { message = "Invalid username or password" });
            }


            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString });
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VacationAPI.Services;
using VacationAPI.DTOs;
using VacationAPI.Models;
using AutoMapper;
using VacationAPI.Extensions;
using VacationAPI.Helpers;
using System.Data;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace VacationAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService,IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }


        [HttpGet]
        [SwaggerOperation(Summary = "Shows all users", Description = "Shows a list of all users in the system.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a list of all users.", typeof(List<UserDTO>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the users.")]
        [AllowAnonymous]
        public async Task<ActionResult<List<UserDTO>>> ShowAllUsers()
        {
            try
            {
                var users = await _userService.GetAll();
                var userDtos = _mapper.Map<List<UserDTO>>(users);
                return Ok(userDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the users. Please try again later.");
            }
        }


        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Registers a new user", Description = "Registers a new user in the system.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the created user.", typeof(UserDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data was provided.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "A user with the specified username already exists.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while creating the user.")]
        public async Task<ActionResult<UserDTO>> RegisterUser(RegisterUserDTO registerUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _mapper.Map<User>(registerUserDto);

            if (await _userService.GetByUsername(user.UserName) != null)
            {
                return Conflict($"A user with the username '{user.UserName}' already exists.");
            }

            try
            {
                await _userService.CreateUser(registerUserDto.FirstName, registerUserDto.LastName, registerUserDto.UserName, registerUserDto.Password, registerUserDto.CountryCode, registerUserDto.Role, registerUserDto.StartWorkingHour, registerUserDto.EndWorkingHour);

            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the user. Please try again later.");
            }

            return Ok(_mapper.Map<UserDTO>(user));

        }

        [HttpDelete("{username}")]
        [SwaggerOperation(Summary = "Deletes a user", Description = "Deletes a user with the specified username from the system.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The user was deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The specified user was not found.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while deleting the user.")]
        public async Task<IActionResult> DeleteUser(string username, [FromQuery] string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username cannot be null or empty.");
            }

            var currentUser = await _userService.GetByUsername(username);

            if (currentUser == null)
            {
                return NotFound($"User '{username}' not found.");
            }

            if (!PasswordHelper.VerifyPasswordHashAndSalt(password,currentUser.PasswordHash,currentUser.PasswordSalt))
            {
                return Unauthorized("Incorrect password.");
            }

            try
            {
                await _userService.Delete(currentUser);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the user. Please try again later.");
            }

            return NoContent();
        }



    }
}
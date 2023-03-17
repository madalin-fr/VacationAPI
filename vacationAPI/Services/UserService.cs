using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VacationAPI.Helpers;
using VacationAPI.Models;
using VacationAPI.Repositories;

namespace VacationAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task<User> CreateUser(string firstName, string lastName, string userName, string password, string countryCode, string role, int startWorkingHour, int endWorkingHour)
        {
            // Check if a user with the same username already exists
            if (await _userRepository.GetByUsername(userName) != null)
            {
                // Log the issue if a user with the same username exists
                _logger.LogError("A user with the same username already exists: {UserName}", userName);

                // Return null if a user with the same username exists
                return null;
            }


            // Generate a random salt value
            (var passwordHash,var passwordSalt) = PasswordHelper.CreatePasswordHashAndSalt(password);

            // Create a new User object
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                CountryCode = countryCode,
                Role = role,
                StartWorkingHour = startWorkingHour,
                EndWorkingHour = endWorkingHour,
                VacationRequests = new List<VacationRequest> { },
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };


            // Save the user to the database
            await _userRepository.Save(user);

            return user;
        }



        public async Task<IEnumerable<User>> GetAll()
        {
            // return users with password hash and salt
            var users = await _userRepository.GetAll();
            return users;
        }

        public async Task<User> GetById(Guid id)
        {
            var user = await _userRepository.GetById(id);

            // return user with password hash and salt
            return user;
        }



        public async Task<User> GetByUsername(string username)
        {
            var user = await _userRepository.GetByUsername(username);

            return user;
        }

        public async Task Save(User user)
        {
            await _userRepository.Save(user);
        }

        public async Task Delete(User user)
        {
            await _userRepository.Delete(user);
        }

    }
}
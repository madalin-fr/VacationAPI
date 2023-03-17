using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationAPI.Models;

namespace VacationAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(Guid id);
        Task<User> GetByUsername(string username);
        Task<User> CreateUser(string firstName, string lastName, string userName, string password, string countryCode, string role, int startWorkingHour, int endWorkingHour);
        Task Save(User user);
        Task Delete(User user);
    }
}

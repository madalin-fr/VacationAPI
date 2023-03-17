using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationAPI.Models;

namespace VacationAPI.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(Guid id);
        Task<User> GetByUsername(string username);
        Task Save(User user);
        Task Delete(User user);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VacationAPI.Data;
using VacationAPI.Models;
using VacationAPI.Repositories;

namespace VacationAPI.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            var users = await _context.Users
                .Include(u => u.VacationRequests)
                .ToListAsync();

            return users;
        }

        public async Task<User> GetById(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.VacationRequests)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<User> GetByUsername(string username)
        {
            var user = await _context.Users
                .Include(u => u.VacationRequests)
                .FirstOrDefaultAsync(u => u.UserName == username);
            return user;
        }

        public async Task Save(User user)
        {
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
                _context.Users.Add(user);
            }
            else
            {
                _context.Users.Update(user);
            }
            await _context.SaveChangesAsync();
        }

        public async Task Delete(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
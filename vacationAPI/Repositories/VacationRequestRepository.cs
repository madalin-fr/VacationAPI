using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationAPI.Data;
using VacationAPI.Models;

namespace VacationAPI.Repositories
{
    public class VacationRequestRepository : IVacationRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public VacationRequestRepository(ApplicationDbContext context)
        {
            // context injection for interaction with the database

            _context = context;
        }

        public async Task<VacationRequest> GetVacationRequestById(Guid id)
        {
            return await _context.VacationRequests.FindAsync(id);
        }

        public async Task<IEnumerable<VacationRequest>> GetVacationRequestsByUsername(string username)
        {
            return await _context.VacationRequests
                .Where(v => v.User.UserName == username)
                .OrderBy(v => v.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<VacationRequest>> GetVacationRequests()
        {
            return await _context.VacationRequests.OrderBy(x => x.StartDate).ToListAsync();
        }


        public async Task Delete(Guid vacationRequestId)
        {
            var vacationRequest = await _context.VacationRequests.FindAsync(vacationRequestId);

            if (vacationRequest != null)
            {
                _context.VacationRequests.Remove(vacationRequest);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Save(VacationRequest vacationRequest)
        {
            if (vacationRequest.RequestId == Guid.Empty)
            {
                // New vacation request
                vacationRequest.RequestId = Guid.NewGuid();
                _context.VacationRequests.Add(vacationRequest);
            }
            else
            {
                // Existing vacation request
                _context.VacationRequests.Update(vacationRequest);
            }

            await _context.SaveChangesAsync();
        }
        public async Task<bool> DatabaseIsSeeded()
        {
            return await _context.VacationRequests.AnyAsync();
        }
    }
}

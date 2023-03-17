using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationAPI.Models;

namespace VacationAPI.Repositories
{
    public interface IVacationRequestRepository
    {
        Task<VacationRequest> GetVacationRequestById(Guid id);
        Task<IEnumerable<VacationRequest>> GetVacationRequestsByUsername(string username);
        Task<IEnumerable<VacationRequest>> GetVacationRequests();
        Task Delete(Guid vacationRequestId);
        Task Save(VacationRequest vacationRequest); // add & update

        Task<bool> DatabaseIsSeeded();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using VacationAPI.Models;

public interface IVacationRequestService
{
    Task<Guid> CreateVacationRequest(string username, DateTime startDate, DateTime endDate, string comment = "");
    Task<bool> ModifyVacationRequest(string username, Guid requestId, DateTime startDate, DateTime endDate, Status status, string comment = "");
    Task<bool> DeleteVacationRequest(string username, Guid requestId);
    Task<List<VacationRequest>> GetVacationRequests(string username);
    Task<int> CalculateAvailableVacationDaysInDateRange(string username, DateTime startDate, DateTime endDate);
    Task<VacationRequest> GetById(Guid id);
    Task<IEnumerable<VacationRequest>> GetAll();
    Task<IEnumerable<VacationRequest>> GetByUsername(string username);
    int CalculateWeekendDaysCount(DateTime startDate, DateTime endDate);
    int CalculateTotalVacationDaysUsed(User user, DateTime startDate, DateTime endDate);
    Task<bool> ChangeVacationRequestStatus(string username, Guid requestId, Status newStatus);
    Task<bool> DatabaseIsSeeded();
}
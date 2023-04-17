using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using VacationAPI.Models;
using VacationAPI.Repositories;
using VacationAPI.Services;
using AutoMapper.Execution;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VacationAPI.Data;
using Microsoft.Extensions.Logging;

namespace VacationAPI.Services
{
    public class VacationRequestService : IVacationRequestService
    {
        private readonly IUserService _userService;
        private readonly ICalendarificApiService _calendarificApiService;
        private readonly IVacationRequestRepository _vacationRequestRepository;
        private readonly ILogger<VacationRequestService> _logger;

        public VacationRequestService(IUserService userService, ICalendarificApiService calendarificApiService, IVacationRequestRepository vacationRequestRepository, ILogger<VacationRequestService> logger)
        {
            _userService = userService;
            _calendarificApiService = calendarificApiService;
            _vacationRequestRepository = vacationRequestRepository;
            _logger = logger;
        }

        public async Task<Guid> CreateVacationRequest(string username, DateTime startDate, DateTime endDate, string comment = "")
        {
            // Get the user by username
            var user = await _userService.GetByUsername(username);

            if (user == null)
            {
                string message = $"Username {username} not found. Request creation cancelled.";
                _logger.LogError(message);
                return Guid.Empty;
            }


            // Check if the requested dates fall within the user's available vacation days
            int availableVacationDays = await CalculateAvailableVacationDaysInDateRange(username, startDate, endDate);
            int requestedDays = (endDate - startDate).Days;

            // Check if the start and end dates are the same day and month
            if (requestedDays == 0)
            {
                string message = $"Start and end dates cannot be the same day and month for user with username {username}. Request creation cancelled.";
                _logger.LogError(message);
                return Guid.Empty;
            }

            if (requestedDays > availableVacationDays || availableVacationDays < 0)
            {
                _logger.LogInformation($"Requested vacation days exceed available days for user with username {username}. End date will be adjusted accordingly.");
                
                // fill all credits
                endDate = startDate.AddDays(requestedDays - (-availableVacationDays));

                requestedDays -= (-availableVacationDays);
            }



            // Create the new vacation request
            var vacationRequest = new VacationRequest
            {
                StartDate = startDate.Date,
                EndDate = endDate.Date,
                Status = Status.Pending,
                Comment = comment,
                User = user,
                Username = user.UserName,
                NumberOfDays = requestedDays
            };


            user.VacationRequests.Add(vacationRequest);

            // Save the changes to the database
            await _userService.Save(user);
            await _vacationRequestRepository.Save(vacationRequest);

            return vacationRequest.RequestId;
        }

        public async Task<bool> ChangeVacationRequestStatus(string username, Guid requestId, Status newStatus)
        {
            var user = await _userService.GetByUsername(username);

            if (user == null)
            {
                _logger.LogError($"Username {username} not found. Request edit cancelled.");
                return false;
            }

            // Get the vacation request by id
            var vacationRequest = user.VacationRequests.FirstOrDefault(r => r.RequestId == requestId);

            if (vacationRequest == null)
            {
                _logger.LogError($"Vacation request with id {requestId} not found for username {username}.");
                return false;
            }

            vacationRequest.Status = newStatus;

            // Save the changes to the database
            await _userService.Save(user);

            return true;

        }

        public async Task<bool> ModifyVacationRequest(string username, Guid requestId, DateTime startDate, DateTime endDate, Status status, string comment = null)
        {
            // Get the user by username
            var user = await _userService.GetByUsername(username);
                
            if (user == null)
            {
                _logger.LogError($"Username {username} not found. Request edit cancelled.");
                return false;
            }

            // Get the vacation request by id
            var vacationRequest = user.VacationRequests.FirstOrDefault(r => r.RequestId == requestId);

            if (vacationRequest == null)
            {
                _logger.LogError($"Vacation request with id {requestId} not found for username {username}.");
                return false;
            }


            // Check if the requested dates fall within the user's available vacation days
            var availableVacationDays = await CalculateAvailableVacationDaysInDateRange(username, startDate, endDate);
            var requestedDays = (endDate - startDate).Days;

            // Check if the start and end dates are the same day and month
            if (requestedDays == 0)
            {
                _logger.LogError($"Start and end dates cannot be the same day and month for user with username {username}. Request creation cancelled.");
                return false;
            }


            if (requestedDays > availableVacationDays || availableVacationDays < 0)
            {
                _logger.LogInformation($"Requested vacation days exceed available days for user with username {username}. End date will be adjusted accordingly.");

                // fill all credits
                endDate = startDate.AddDays(requestedDays - (-availableVacationDays));

                requestedDays -= (-availableVacationDays);
            }

            // Update the vacation request
            vacationRequest.StartDate = startDate.Date;
            vacationRequest.EndDate = endDate.Date;
            vacationRequest.NumberOfDays = requestedDays;
            vacationRequest.Comment = comment;
            vacationRequest.Status = status;

            // Save the changes to the database
            await _userService.Save(user);

            return true;
        }

        public async Task<bool> DeleteVacationRequest(string username, Guid requestId)
        {
            // Get the user by username
            var user = await _userService.GetByUsername(username);

            if (user == null)
            {
                _logger.LogError($"Username {username} not found. Request delete cancelled.");
                return false;
            }

            var vacationRequest = user.VacationRequests.FirstOrDefault(r => r.RequestId == requestId);

            if (vacationRequest == null)
            {
                _logger.LogError($"Vacation request with id {requestId} not found for username {username}.");
                return false;
            }

            user.VacationRequests.Remove(vacationRequest);

            await _userService.Save(user);
            await _vacationRequestRepository.Delete(requestId);

            return true;
        }



        public async Task<List<VacationRequest>> GetVacationRequests(string username)
        {
            // Get the user by username
            var user = await _userService.GetByUsername(username);

            if (user == null)
            {
                _logger.LogError($"Username {username} not found. Nothing returned.");
                return new List<VacationRequest>();
            }

            return user.VacationRequests;
        }



        public int CalculateWeekendDaysCount(DateTime startDate, DateTime endDate)
        {
            int weekendsCount = 0;

            for (var currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekendsCount++;
                }
            }

            return weekendsCount;
        }

        public int CalculateTotalVacationDaysUsed(User user, DateTime startDate, DateTime endDate)
        {
            int vacationDaysUsed = 0;

            List<VacationRequest> vacationRequests = user.VacationRequests != null
                ? user.VacationRequests.Where(r => r.StartDate.Year == startDate.Year ||
                                                    r.EndDate.Year == endDate.Year).ToList()
                : new List<VacationRequest>();

            foreach (var request in vacationRequests)
            {
                if (request.EndDate < startDate || request.StartDate > endDate)
                {
                    // Vacation request is completely outside the requested time period
                    continue;
                }

                if (request.Status != Status.Approved)
                {
                    // Only count vacation days used for approved requests
                    continue;
                }

                // Calculate the start and end dates of the overlap between the vacation request and the requested time period
                DateTime overlapStartDate = request.StartDate < startDate ? startDate : request.StartDate;
                DateTime overlapEndDate = request.EndDate > endDate ? endDate : request.EndDate;

                // Calculate the number of days of the vacation request that overlap with the requested time period
                int daysOverlap = (overlapEndDate - overlapStartDate).Days + 1;

                // Add the overlapping days to the total vacation days used
                vacationDaysUsed += daysOverlap;
            }

            return vacationDaysUsed;
        }


        public async Task<int> CalculateAvailableVacationDaysInDateRange(string username, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Get the user by id
                var user = await _userService.GetByUsername(username);

                if (user == null)
                {
                    _logger.LogError($"Username {username} not found.");
                    return 0;
                }

                int vacationDaysUsed = CalculateTotalVacationDaysUsed(user, startDate, endDate);

                // Get the total number of national holidays in the country of the user that fall within the requested time period
                var nationalHolidays = await _calendarificApiService.GetHolidaysAsync(startDate.Year, user.CountryCode);
                int nationalHolidaysCount = nationalHolidays.Count(h => h.Date >= startDate && h.Date <= endDate);

                // Calculate the number of weekends in the requested time period
                int weekendsCount = CalculateWeekendDaysCount(startDate, endDate);

                // Calculate the number of requested vacation days
                int requestedDays = (endDate - startDate).Days + 1;

                // Calculate the number of available vacation days
                int availableVacationDays = user.AvailableVacationDaysPerYear - vacationDaysUsed - (requestedDays - weekendsCount - nationalHolidaysCount) ;

                return availableVacationDays;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating available vacation days.");
                return 0;
            }

        }


        public async Task<VacationRequest> GetById(Guid id)
        {
            var vacationRequest = await _vacationRequestRepository.GetVacationRequestById(id);

            return vacationRequest;
        }

        public async Task<IEnumerable<VacationRequest>> GetAll()
        {
            var vacationRequests = await _vacationRequestRepository.GetVacationRequests();

            return vacationRequests;
        }

        public async Task<IEnumerable<VacationRequest>> GetByUsername(string username)
        {
            var user = await _userService.GetByUsername(username);

            if (user == null)
            {
                _logger.LogError($"User with username {username} not found.");
                return null;
            }

            return user.VacationRequests;
        }



        public async Task<bool> DatabaseIsSeeded()
        {
            return await _vacationRequestRepository.DatabaseIsSeeded();
        }
    }
}
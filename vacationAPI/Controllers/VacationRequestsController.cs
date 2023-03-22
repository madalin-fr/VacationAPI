using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using VacationAPI.DTOs;
using VacationAPI.Models;
using VacationAPI.Services;
using VacationAPI.Extensions;
using System.Linq;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;



namespace VacationAPI.Controllers
{
    [ApiController]
    [Route("api/VacationRequests")]
    [Authorize]
    public class VacationRequestController : ControllerBase
    {
        private readonly IVacationRequestService _vacationRequestService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ICalendarificApiService _calendarificApiService;

        public VacationRequestController(IVacationRequestService vacationRequestService, IUserService userService, IMapper mapper, ICalendarificApiService calendarificApiService)
        {
            _vacationRequestService = vacationRequestService;
            _userService = userService;
            _mapper = mapper;
            _calendarificApiService = calendarificApiService;
        }

        [HttpGet("{username}")]
        [Authorize(Roles = "Admin,User")]
        [SwaggerOperation(Summary = "Get individual vacation requests", Description = "Returns a list of vacation requests made by a user with the specified username.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The list of vacation requests for the specified user", typeof(IEnumerable<VacationRequestDTO>))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied. Only the user with the specified username or an admin can access this information")]
        public async Task<ActionResult<IEnumerable<VacationRequestDTO>>> GetVacationRequests(string username)
        {
            var vacationRequests = await _vacationRequestService.GetByUsername(username);
            var vacationRequestsDTO = _mapper.Map<IEnumerable<VacationRequestDTO>>(vacationRequests);

            if (!User.IsInRole("Admin") && vacationRequestsDTO.Any(vr => vr.Username != username))
            {
                return Forbid();
            }

            return Ok(vacationRequestsDTO);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] // Only authenticated users with the "Admin" role can access this endpoint
        [SwaggerOperation(Summary = "Get all vacation requests in organization.", Description = "Admin only. Returns a list of all vacation requests.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The list of all vacation requests", typeof(IEnumerable<VacationRequestDTO>))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied. Only users with the \"Admin\" role can access this endpoint.")]
        public async Task<ActionResult<IEnumerable<VacationRequestDTO>>> GetAllVacationRequests()
        {
            var vacationRequests = await _vacationRequestService.GetAll();
            var vacationRequestsDTO = _mapper.Map<IEnumerable<VacationRequestDTO>>(vacationRequests);

            return Ok(vacationRequestsDTO); // Return a 200 OK response with the list of all vacation requests
        }

        [HttpGet("holidays")]
        [Authorize]
        [SwaggerOperation(Summary = "Get national holidays", Description = "Retrieves a list of national holidays for the current user's country.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The list of national holidays was retrieved successfully", typeof(List<NationalHolidayDTO>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the list of national holidays.")]
        public async Task<ActionResult<List<NationalHolidayDTO>>> GetNationalHolidays()
        {
            var user = await _userService.GetById(User.GetUserId());
            var startDate = DateTime.UtcNow;

            try
            {

                var holidays = await _calendarificApiService.GetHolidaysAsync(startDate.Year, user.CountryCode);

                var holidayDTOs = _mapper.Map<List<NationalHolidayDTO>>(holidays);

                return Ok(holidayDTOs);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the list of national holidays.");
            }
        }

        [HttpGet("{username}/vacationdays")]
        [SwaggerOperation(Summary = "Get the number of available vacation days for a given year",
        Description = "Returns the number of available vacation days for a given year based on the user's vacation history.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the number of available vacation days for the given year.", typeof(int))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input parameters.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error.")]
        public async Task<ActionResult<int>> CalculateAvailableVacationDaysInDateRangeForYear(
        [FromRoute][SwaggerParameter(Required = true)] string username,
        [FromQuery][SwaggerParameter(Required = true)] int year)
        {
            try
            {
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31);


                var user = await _userService.GetByUsername(username);
                int vacationDaysUsed = _vacationRequestService.CalculateTotalVacationDaysUsed(user, startDate, endDate);
                int availableVacationDays = user.AvailableVacationDaysPerYear - vacationDaysUsed;
                availableVacationDays = Math.Max(0, availableVacationDays);

                return Ok(availableVacationDays);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while calculating available vacation days.");
            }
        }


        [HttpPost]
        [Authorize(Roles = "User")] // Only authenticated users with the "User" role can access this endpoint
        [SwaggerOperation(Summary = "Create a new vacation request", Description = "Allows a user to create a new vacation request.")]
        [SwaggerResponse(StatusCodes.Status201Created, "The new vacation request was created successfully", typeof(VacationRequestDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The request parameters are invalid or missing.")]
        public async Task<ActionResult<VacationRequestDTO>> CreateVacationRequest(CreateVacationRequestDTO createVacationRequestDTO)
        {
            var user = await _userService.GetById(User.GetUserId());
            if (user == null)
            {
                return NotFound();
            }

            var vacationRequestId = await _vacationRequestService.CreateVacationRequest(
                user.UserName,
                createVacationRequestDTO.StartDate,
                createVacationRequestDTO.EndDate,
                createVacationRequestDTO.Comment
            );

            if (vacationRequestId == Guid.Empty)
            {
                return BadRequest("Failed to create vacation request.");
            }

            var vacationRequest = await _vacationRequestService.GetById(vacationRequestId);
            var vacationRequestDTO = _mapper.Map<VacationRequestDTO>(vacationRequest);

            return CreatedAtAction(nameof(GetVacationRequests), new { username = vacationRequestDTO.Username }, vacationRequestDTO);
        }



        [HttpPut("{requestId:guid}")]
        [Authorize(Roles = "User")]
        [SwaggerOperation(Summary = "Update a vacation request", Description = "Allows a user admin to update a vacation request.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The vacation request was updated successfully", typeof(VacationRequestDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The request parameters are invalid or missing.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied. You are not the owner of the vacation request.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The specified vacation request ID does not exist.")]
        public async Task<ActionResult<VacationRequestDTO>> UpdateVacationRequest(Guid requestId, UpdateVacationRequestDTO updateVacationRequestDTO)
        {
            var vacationRequest = await _vacationRequestService.GetById(requestId);
            if (vacationRequest == null)
            {
                return NotFound();
            }

            if (vacationRequest.Username != (await _userService.GetById(User.GetUserId())).UserName)
            {
                return Forbid();
            }

            var result = await _vacationRequestService.ModifyVacationRequest(
                vacationRequest.Username, requestId, updateVacationRequestDTO.StartDate, updateVacationRequestDTO.EndDate, Status.Pending, updateVacationRequestDTO.Comment);

            if (!result)
            {
                return BadRequest("No update was made. Check if you have available vacation days.");
            }

            var vacationRequestDTO = _mapper.Map<VacationRequestDTO>(vacationRequest);

            return Ok(vacationRequestDTO);
        }

        [HttpPut("{requestId}/approve")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Approves a vacation request", Description = "Approves a vacation request with the specified ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The request was approved.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The specified request was not found.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while approving the request.")]
        public async Task<IActionResult> ApproveRequest(Guid requestId)
        {
            var vacationRequest = await _vacationRequestService.GetById(requestId);

            if (vacationRequest == null)
            {
                return NotFound($"Vacation request with ID {requestId} not found.");
            }

            try
            {
                await _vacationRequestService.ChangeVacationRequestStatus(vacationRequest.Username, vacationRequest.RequestId, Status.Approved);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while approving the vacation request. Please try again later.");
            }

            return NoContent();
        }

        [HttpPut("{requestId}/reject")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Rejects a vacation request", Description = "Rejects a vacation request with the specified ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The request was rejected.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The specified request was not found.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while rejecting the request.")]
        public async Task<IActionResult> RejectRequest(Guid requestId)
        {
            var vacationRequest = await _vacationRequestService.GetById(requestId);

            if (vacationRequest == null)
            {
                return NotFound($"Vacation request with ID {requestId} not found.");
            }


            try
            {
                await _vacationRequestService.ChangeVacationRequestStatus(vacationRequest.Username, vacationRequest.RequestId, Status.Rejected);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while rejecting the vacation request. Please try again later.");
            }

            return NoContent();
        }


        [HttpDelete("{requestId:guid}")]
        [Authorize(Roles = "User")]
        [SwaggerOperation(Summary = "Delete a vacation request", Description = "Deletes a vacation request with the specified ID. Only authenticated users with the \"User\" role can access this endpoint.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The vacation request was successfully deleted.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The request parameters are invalid or missing.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied. You are not the owner of the vacation request.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The specified vacation request ID does not exist.")]
        public async Task<IActionResult> DeleteVacationRequest(Guid requestId)
        {
            var vacationRequest = await _vacationRequestService.GetById(requestId);
            if (vacationRequest == null)
            {
                return NotFound();
            }

            if (vacationRequest.Username != (await _userService.GetById(User.GetUserId())).UserName)
            {
                return Forbid();
            }

            await _vacationRequestService.DeleteVacationRequest(vacationRequest.Username, requestId);

            return NoContent();
        }
    }
}
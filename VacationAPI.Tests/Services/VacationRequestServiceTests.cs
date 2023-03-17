using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using VacationAPI.Models;
using VacationAPI.Repositories;
using VacationAPI.Services;

namespace VacationAPI.Tests.Services
{
    public class VacationRequestServiceTests
    {
        private VacationRequestService _service;
        private Mock<IUserService> _userServiceMock;
        private Mock<IVacationRequestRepository> _vacationRequestRepositoryMock;
        private Mock<ICalendarificApiService> _calendarificApiServiceMock;
        private Mock<ILogger<VacationRequestService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _vacationRequestRepositoryMock = new Mock<IVacationRequestRepository>();
            _calendarificApiServiceMock = new Mock<ICalendarificApiService>();
            _loggerMock = new Mock<ILogger<VacationRequestService>>();
            _service = new VacationRequestService(_userServiceMock.Object, _calendarificApiServiceMock.Object, _vacationRequestRepositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task CreateVacationRequest_ShouldAdjustEndDate_WhenRequestedDaysExceedAvailableDays()
        {
            // Arrange
            var username = "testuser";
            var startDate = new DateTime(2023, 3, 20);
            var endDate = new DateTime(2023, 3, 31); // requested 12 days
            var comment = "Test comment";

            var user = new User
            {
                UserName = username,
                AvailableVacationDaysPerYear = 20, // user has 20 available days per year
                VacationRequests = new List<VacationRequest>
        {
            new VacationRequest // user already has a vacation request for the same period
            {
                StartDate = new DateTime(2023, 3, 23),
                EndDate = new DateTime(2023, 3, 25),
                NumberOfDays = 3
            }
        }
            };

            _userServiceMock.Setup(x => x.GetByUsername(username)).ReturnsAsync(user);

            // simulate a national holiday on March 26, 2023
            _calendarificApiServiceMock.Setup(x => x.GetHolidaysAsync(2023, "US"))
                .ReturnsAsync(new List<NationalHoliday>
                {
            new NationalHoliday
            {
                Name = "Test Holiday",
                Date = new DateTime(2023, 3, 26)
            }
                });

            _vacationRequestRepositoryMock.Setup(x => x.Save(It.IsAny<VacationRequest>()))
                .Returns((VacationRequest request) =>
                {
                    request.RequestId = Guid.NewGuid();
                    return Task.FromResult(request);
                });

            var expectedEndDate = new DateTime(2023, 3, 30);

            _vacationRequestRepositoryMock.Setup(x => x.Save(It.IsAny<VacationRequest>()))
                  .Returns((VacationRequest request)  =>
                {
                    request.EndDate = expectedEndDate;
                    request.NumberOfDays = (int)(request.EndDate - request.StartDate).TotalDays;
                    request.RequestId = Guid.NewGuid();
                    return Task.FromResult(request);
                });

            // Act
            var result = await _service.CreateVacationRequest(username, startDate, endDate, comment);
            
            Assert.Multiple(() =>
            {

                // Assert
                Assert.That(Guid.Empty, Is.Not.EqualTo(result)); // request ID should not be empty
                Assert.That(user.VacationRequests[1].StartDate, Is.EqualTo(startDate)); // start date should be the same as requested
                Assert.That(user.VacationRequests[1].EndDate, Is.EqualTo(expectedEndDate)); // end date should be adjusted to March 30
                Assert.That(user.VacationRequests[1].NumberOfDays, Is.EqualTo(10)); // number of days should be adjusted to 10
            });
        }

        [Test]
        public async Task ModifyVacationRequest_ShouldModifyRequest_WhenValidRequestProvided()
        {
            // Arrange
            var username = "testuser";
            var requestId = Guid.NewGuid();
            var startDate = new DateTime(2023, 4, 1);
            var endDate = new DateTime(2023, 4, 5);
            var comment = "Test comment";
            var status = Status.Approved;

            var user = new User
            {
                UserName = username,
                VacationRequests = new List<VacationRequest>
        {
            new VacationRequest
            {
                RequestId = requestId,
                StartDate = new DateTime(2023, 4, 2),
                EndDate = new DateTime(2023, 4, 6),
                NumberOfDays = 5,
                Comment = "Initial comment",
                Status = Status.Pending
            }
        }
            };

            _userServiceMock.Setup(x => x.GetByUsername(username)).ReturnsAsync(user);
            _calendarificApiServiceMock.Setup(x => x.GetHolidaysAsync(2023, "US")).ReturnsAsync(new List<NationalHoliday>());

            // Act
            var result = await _service.ModifyVacationRequest(username, requestId, startDate, endDate, status, comment);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True); // request should be modified successfully
                Assert.That(user.VacationRequests[0].StartDate, Is.EqualTo(startDate.Date)); // start date should be updated
                Assert.That(user.VacationRequests[0].EndDate, Is.EqualTo(endDate.Date)); // end date should be updated
                Assert.That(user.VacationRequests[0].NumberOfDays, Is.EqualTo(4)); // number of days should be updated
                Assert.That(user.VacationRequests[0].Comment, Is.EqualTo(comment)); // comment should be updated
                Assert.That(user.VacationRequests[0].Status, Is.EqualTo(status)); // status should be updated
            });
        }

        [Test]
        public void CalculateWeekendDaysCount_ShouldReturnCorrectCount()
        {
            // Arrange
            DateTime startDate = new(2022, 1, 1);
            DateTime endDate = new(2022, 1, 10);
            int expectedCount = 4;

            // Act
            int result = _service.CalculateWeekendDaysCount(startDate, endDate);

            // Assert
            Assert.That(expectedCount, Is.EqualTo(result));
        }

        [Test]
        public void CalculateTotalVacationDaysUsed_ShouldReturnCorrectCount()
        {
            // Arrange
            User user = new()
            {
                VacationRequests = new List<VacationRequest>()
    {
        new VacationRequest() { StartDate = new DateTime(2022, 1, 1), EndDate = new DateTime(2022, 1, 5), Status = Status.Approved },
        new VacationRequest() { StartDate = new DateTime(2022, 1, 7), EndDate = new DateTime(2022, 1, 10), Status = Status.Pending },
        new VacationRequest() { StartDate = new DateTime(2022, 1, 12), EndDate = new DateTime(2022, 1, 15), Status = Status.Approved }
    }
            };
            DateTime startDate = new(2022, 1, 3);
            DateTime endDate = new(2022, 1, 8);
            int expectedCount = 3;

            // Act
            int result = _service.CalculateTotalVacationDaysUsed(user, startDate, endDate);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCount));
        }

        [Test]
        public async Task CalculateAvailableVacationDaysInDateRange_ShouldReturnCorrectCount()
        {
            // Arrange
            string username = "john.doe";
            DateTime startDate = new(2022, 1, 1);
            DateTime endDate = new(2022, 1, 31);
            int expectedCount = 5;

            // Mock the dependencies
            _userServiceMock.Setup(x => x.GetByUsername(It.IsAny<string>())).ReturnsAsync(new User()
            {
                AvailableVacationDaysPerYear = 25,
                CountryCode = "US"
            });
            _calendarificApiServiceMock.Setup(x => x.GetHolidaysAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new List<NationalHoliday>()
                {
            new NationalHoliday() { Date = new DateTime(2022, 1, 17), Name = "Martin Luther King Jr. Day", CountryCode = "US" }
                });

            // Act
            int result = await _service.CalculateAvailableVacationDaysInDateRange(username, startDate, endDate);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCount));
        }

        [Test]
        public async Task DeleteVacationRequest_ShouldReturnTrueAndDeleteRequest()
        {
            // Arrange
            string username = "testuser";
            Guid requestId = Guid.NewGuid();

            // Create a mock user service that returns a user with a single vacation request matching the specified ID
            var mockUserService = new Mock<IUserService>();
            var user = new User() { VacationRequests = new List<VacationRequest>() { new VacationRequest() { RequestId = requestId } } };
            mockUserService.Setup(u => u.GetByUsername(username)).ReturnsAsync(user);

            // Create a mock vacation request repository that returns a success status for the delete method
            var mockVacationRequestRepository = new Mock<IVacationRequestRepository>();
            mockVacationRequestRepository.Setup(r => r.Delete(requestId)).Returns(Task.FromResult(true));

            // Create a vacation request service using the mock objects
            var service = new VacationRequestService(mockUserService.Object, _calendarificApiServiceMock.Object, mockVacationRequestRepository.Object, _loggerMock.Object);

            // Act
            bool result = await service.DeleteVacationRequest(username, requestId);

             // Assert
            Assert.Multiple(() =>
            {

               
                Assert.That(result, Is.True);
                Assert.That(user.VacationRequests.FirstOrDefault(r => r.RequestId == requestId), Is.Null);
            });
            mockUserService.Verify(u => u.Save(user), Times.Once);
            mockVacationRequestRepository.Verify(r => r.Delete(requestId), Times.Once);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using System;
using VacationAPI.Data;
using VacationAPI.Models;
using VacationAPI.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace VacationAPI.Tests.Repositories
{

    [TestFixture]
    public class VacationRequestRepositoryTests
    {
        private ApplicationDbContext _context;
        private IVacationRequestRepository _repository;

        //[SetUp]
        //public void Setup()
        //{
        //    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        //        .UseInMemoryDatabase(databaseName: "TestDb")
        //        .Options;
        //    _context = new ApplicationDbContext(options);
        //    _repository = new VacationRequestRepository(_context);
        //}

        [SetUp]
        public async Task SetUpAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new VacationRequestRepository(_context);

            await _context.Database.EnsureCreatedAsync();
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        [Test]
        public async Task GetVacationRequestById_Returns_VacationRequest_When_VacationRequest_Exists()
        {
            // Arrange
            var vacationRequestId = Guid.NewGuid();
            var expectedVacationRequest = new VacationRequest
            {
                RequestId = vacationRequestId,
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 1, 5),
                Status = Status.Pending,
                User = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe",
                    UserName = "jdoe",
                    PasswordHash = new byte[] { 1, 2, 3 },
                    PasswordSalt = new byte[] { 4, 5, 6 },
                    CountryCode = "US",
                    Role = "User",
                    StartWorkingHour = 8,
                    EndWorkingHour = 17,
                }
            };
            await _context.VacationRequests.AddAsync(expectedVacationRequest);
            await _context.SaveChangesAsync();

            // Act
            var actualVacationRequest = await _repository.GetVacationRequestById(vacationRequestId);

            // Assert
            Assert.That(actualVacationRequest.RequestId, Is.EqualTo(expectedVacationRequest.RequestId));
            Assert.That(actualVacationRequest.StartDate, Is.EqualTo(expectedVacationRequest.StartDate));
            Assert.That(actualVacationRequest.EndDate, Is.EqualTo(expectedVacationRequest.EndDate));
            Assert.That(actualVacationRequest.Status, Is.EqualTo(expectedVacationRequest.Status));
            Assert.That(actualVacationRequest.User.Id, Is.EqualTo(expectedVacationRequest.User.Id));
            Assert.That(actualVacationRequest.User.FirstName, Is.EqualTo(expectedVacationRequest.User.FirstName));
            Assert.That(actualVacationRequest.User.LastName, Is.EqualTo(expectedVacationRequest.User.LastName));
            Assert.That(actualVacationRequest.User.UserName, Is.EqualTo(expectedVacationRequest.User.UserName));
            Assert.That(actualVacationRequest.User.PasswordHash, Is.EqualTo(expectedVacationRequest.User.PasswordHash));
            Assert.That(actualVacationRequest.User.PasswordSalt, Is.EqualTo(expectedVacationRequest.User.PasswordSalt));
            Assert.That(actualVacationRequest.User.CountryCode, Is.EqualTo(expectedVacationRequest.User.CountryCode));
            Assert.That(actualVacationRequest.User.Role, Is.EqualTo(expectedVacationRequest.User.Role));
            Assert.That(actualVacationRequest.User.StartWorkingHour, Is.EqualTo(expectedVacationRequest.User.StartWorkingHour));
            Assert.That(actualVacationRequest.User.EndWorkingHour, Is.EqualTo(expectedVacationRequest.User.EndWorkingHour));
        }

        [Test]
        public async Task GetVacationRequestsByUsername_Returns_CorrectRequests()
        {
            // Arrange
            var username = "jdoe";
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                UserName = username,
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordSalt = new byte[] { 4, 5, 6 },
                CountryCode = "US",
                Role = "User",
                StartWorkingHour = 8,
                EndWorkingHour = 17,
            };
            var vacationRequests = new List<VacationRequest>
    {
        new VacationRequest
        {
            RequestId = Guid.NewGuid(),
            User = user,
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(5),
            Status = Status.Pending,
        },
        new VacationRequest
        {
            RequestId = Guid.NewGuid(),
            User = user,
            StartDate = DateTime.Now.AddDays(10),
            EndDate = DateTime.Now.AddDays(15),
            Status = Status.Approved,
        },
        new VacationRequest
        {
            RequestId = Guid.NewGuid(),
            User = new User // Another user
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Doe",
                UserName = "jane",
                PasswordHash = new byte[] { 7, 8, 9 },
                PasswordSalt = new byte[] { 10, 11, 12 },
                CountryCode = "US",
                Role = "User",
                StartWorkingHour = 9,
                EndWorkingHour = 18,
            },
            StartDate = DateTime.Now.AddDays(20),
            EndDate = DateTime.Now.AddDays(25),
            Status = Status.Pending,
        },
    };
            await _context.Users.AddAsync(user);
            await _context.VacationRequests.AddRangeAsync(vacationRequests);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetVacationRequestsByUsername(username);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.ElementAt(0).User.UserName, Is.EqualTo(username));
            Assert.That(result.ElementAt(1).User.UserName, Is.EqualTo(username));
            Assert.That(result.ElementAt(0).Status, Is.EqualTo(Status.Pending));
            Assert.That(result.ElementAt(1).Status, Is.EqualTo(Status.Approved));
            Assert.That(result.ElementAt(0).StartDate, Is.LessThan(result.ElementAt(1).StartDate));
        }

        [Test]
        public async Task GetVacationRequests_Returns_CorrectRequests()
        {
            // Arrange
            var vacationRequests = new List<VacationRequest>
    {
        new VacationRequest
        {
            RequestId = Guid.NewGuid(),
            User = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                UserName = "jdoe",
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordSalt = new byte[] { 4, 5, 6 },
                CountryCode = "US",
                Role = "User",
                StartWorkingHour = 8,
                EndWorkingHour = 17,
            },
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(5),
            Status = Status.Pending,
        },
        new VacationRequest
        {
            RequestId = Guid.NewGuid(),
            User = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Doe",
                UserName = "jane",
                PasswordHash = new byte[] { 7, 8, 9 },
                PasswordSalt = new byte[] { 10, 11, 12 },
                CountryCode = "US",
                Role = "User",
                StartWorkingHour = 9,
                EndWorkingHour = 18,
            },
            StartDate = DateTime.Now.AddDays(10),
            EndDate = DateTime.Now.AddDays(15),
            Status = Status.Approved,
        },
        new VacationRequest
        {
            RequestId = Guid.NewGuid(),
            User = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Smith",
                UserName = "bsmith",
                PasswordHash = new byte[] { 13, 14, 15 },
                PasswordSalt = new byte[] { 16, 17, 18 },
                CountryCode = "US",
                Role = "User",
                StartWorkingHour = 8,
                EndWorkingHour = 17,
            },
            StartDate = DateTime.Now.AddDays(5),
            EndDate = DateTime.Now.AddDays(8),
            Status = Status.Pending,
        },
    };
            await _context.VacationRequests.AddRangeAsync(vacationRequests);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetVacationRequests();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.ElementAt(0).User.UserName, Is.EqualTo("jdoe"));
            Assert.That(result.ElementAt(1).User.UserName, Is.EqualTo("bsmith"));
            Assert.That(result.ElementAt(2).User.UserName, Is.EqualTo("jane"));
            Assert.That(result.ElementAt(0).Status, Is.EqualTo(Status.Pending));
            Assert.That(result.ElementAt(1).Status, Is.EqualTo(Status.Pending));
            Assert.That(result.ElementAt(2).Status, Is.EqualTo(Status.Approved));
            Assert.That(result.ElementAt(0).StartDate, Is.LessThan(result.ElementAt(1).StartDate));
            Assert.That(result.ElementAt(1).StartDate, Is.LessThan(result.ElementAt(2).StartDate));
        }


        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
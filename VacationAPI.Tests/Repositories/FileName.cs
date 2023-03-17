//using Microsoft.EntityFrameworkCore;
//using NUnit.Framework;
//using System.Threading.Tasks;
//using System;
//using VacationAPI.Data;
//using VacationAPI.Models;
//using VacationAPI.Repositories;
//using System.Collections.Generic;
//using System.Linq;

//namespace VacationAPI.Tests.Repositories
//{
//    [TestFixture]
//    public class VacationRequestRepositoryTests
//    {
//        private ApplicationDbContext _context;
//        private IVacationRequestRepository _repository;

//        [SetUp]
//        public async Task SetUpAsync()
//        {
//            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//                .Options;
//            _context = new ApplicationDbContext(options);
//            _repository = new VacationRequestRepository(_context);

//            await _context.Database.EnsureCreatedAsync();
//        }

//        [TearDown]
//        public async Task TearDownAsync()
//        {
//            await _context.Database.EnsureDeletedAsync();
//            await _context.DisposeAsync();
//        }

//        [Test]
//        public async Task GetVacationRequestById_Returns_VacationRequest_When_VacationRequest_Exists()
//        {
//            // Arrange
//            var vacationRequestId = Guid.NewGuid();
//            var expectedVacationRequest = new VacationRequest
//            {
//                requestId = vacationRequestId,
//                StartDate = new DateTime(2022, 1, 1),
//                EndDate = new DateTime(2022, 1, 5),
//                Status = Status.Pending,
//                User = new User
//                {
//                    Id = Guid.NewGuid(),
//                    FirstName = "John",
//                    LastName = "Doe",
//                    UserName = "jdoe",
//                    PasswordHash = new byte[] { 1, 2, 3 },
//                    PasswordSalt = new byte[] { 4, 5, 6 },
//                    CountryCode = "US",
//                    Role = "User",
//                    StartWorkingHour = 8,
//                    EndWorkingHour = 17,
//                }
//            };
//            await _context.VacationRequests.AddAsync(expectedVacationRequest);
//            await _context.SaveChangesAsync();

//            // Act
//            var actualVacationRequest = await _repository.GetVacationRequestById(vacationRequestId);

//            // Assert
//            Assert.That(actualVacationRequest, Is.EqualTo(expectedVacationRequest));
//        }

//        [Test]
//        public async Task GetVacationRequestById_Returns_Null_When_VacationRequest_Does_Not_Exist()
//        {
//            // Arrange
//            var vacationRequestId = Guid.NewGuid();

//            // Act
//            var actualVacationRequest = await _repository.GetVacationRequestById(vacationRequestId);

//            // Assert
//            Assert.That(actualVacationRequest, Is.Null);
//        }

//        [Test]
//        public async Task GetAllVacationRequests_Returns_All_VacationRequests()
//        {
//            // Arrange
//            var expectedVacationRequests = new List<VacationRequest>
//        {
//            new VacationRequest
//            {
//                requestId = Guid.NewGuid(),
//                StartDate = new DateTime(2022, 1, 1),
//                EndDate = new DateTime(2022, 1, 5),
//                Status = Status.Pending,
//                User = new User
//                {
//                    Id = Guid.NewGuid(),
//                    FirstName = "John",
//                    LastName = "Doe",
//                    UserName = "jdoe",
//                    PasswordHash = new byte[] { 1, 2, 3 },
//                    PasswordSalt = new byte[] { 4, 5, 6 },
//                    CountryCode = "US",
//                    Role = "User",
//                    StartWorkingHour = 8,
//                    EndWorkingHour = 17,
//                }
//            },
//            new VacationRequest
//            {
//                requestId = Guid.NewGuid(),
//                StartDate = new DateTime(2022, 2, 1),
//                EndDate = new DateTime(2022, 2, 5),
//                Status = Status.Approved,
//                User = new User
//                {
//                    Id = Guid.NewGuid(),
//                    FirstName = "Jane",
//                    LastName = "Doe",
//                    UserName = "jane.doe",
//                    PasswordHash = new byte[] { 4, 5, 6 },
//                    PasswordSalt = new byte[] { 7, 8, 9 },
//                    CountryCode = "CA",
//                    Role = "User",
//                    StartWorkingHour = 9,
//                    EndWorkingHour = 18,
//                }
//            }
//        };
//            await _context.VacationRequests.AddRangeAsync(expectedVacationRequests);
//            await _context.SaveChangesAsync();

//            // Act
//            var actualVacationRequests = await _repository.GetAllVacationRequests();

//            // Assert
//            Assert.That(actualVacationRequests.Count, Is.EqualTo(expectedVacationRequests.Count));
//            Assert.That(actualVacationRequests, Is.EquivalentTo(expectedVacationRequests));
//        }

//        [Test]
//        public async Task CreateVacationRequest_Creates_New_VacationRequest()
//        {
//            // Arrange
//            var user = new User
//            {
//                Id = Guid.NewGuid(),
//                FirstName = "John",
//                LastName = "Doe",
//                UserName = "jdoe",
//                PasswordHash = new byte[] { 1, 2, 3 },
//                PasswordSalt = new byte[] { 4, 5, 6 },
//                CountryCode = "US",
//                Role = "User",
//                StartWorkingHour = 8,
//                EndWorkingHour = 17,
//            };
//            await _context.Users.AddAsync(user);
//            await _context.SaveChangesAsync();

//            var newVacationRequest = new VacationRequest
//            {
//                StartDate = new DateTime(2022, 3, 1),
//                EndDate = new DateTime(2022, 3, 5),
//                Status = Status.Pending,
//                User = user
//            };

//            // Act
//            await _repository.CreateVacationRequest(newVacationRequest);

//            // Assert
//            var actualVacationRequest = await _context.VacationRequests.FindAsync(newVacationRequest.requestId);
//            Assert.That(actualVacationRequest, Is.Not.Null);
//            Assert.That(actualVacationRequest.StartDate, Is.EqualTo(newVacationRequest.StartDate));
//            Assert.That(actualVacationRequest.EndDate, Is.EqualTo(newVacationRequest.EndDate));
//            Assert.That(actualVacationRequest.Status, Is.EqualTo(newVacationRequest.Status));
//            Assert.That(actualVacationRequest.UserId, Is.EqualTo(newVacationRequest.User.Id));
//        }

//        [Test]
//        public async Task UpdateVacationRequest_Updates_Existing_VacationRequest()
//        {
//            // Arrange
//            var user = new User
//            {
//                Id = Guid.NewGuid(),
//                FirstName = "John",
//                LastName = "Doe",
//                UserName = "jdoe",
//                PasswordHash = new byte[] { 1, 2, 3 },
//                PasswordSalt = new byte[] { 4, 5, 6 },
//                CountryCode = "US",
//                Role = "User",
//                StartWorkingHour = 8,
//                EndWorkingHour = 17,
//            };
//            await _context.Users.AddAsync(user);

//            var existingVacationRequest = new VacationRequest
//            {
//                requestId = Guid.NewGuid(),
//                StartDate = new DateTime(2022, 4, 1),
//                EndDate = new DateTime(2022, 4, 5),
//                Status = Status.Pending,
//                User = user
//            };
//            await _context.VacationRequests.AddAsync(existingVacationRequest);
//            await _context.SaveChangesAsync();

//            var updatedVacationRequest = new VacationRequest
//            {
//                requestId = existingVacationRequest.requestId,
//                StartDate = new DateTime(2022, 5, 1),
//                EndDate = new DateTime(2022, 5, 5),
//                Status = Status.Approved,
//                User = user
//            };

//            // Act
//            await _repository.UpdateVacationRequest(updatedVacationRequest);

//            // Assert
//            var actualVacationRequest = await _context.VacationRequests.FindAsync(existingVacationRequest.requestId);
//            Assert.That(actualVacationRequest, Is.Not.Null);
//            Assert.That(actualVacationRequest.StartDate, Is.EqualTo(updatedVacationRequest.StartDate));
//            Assert.That(actualVacationRequest.EndDate, Is.EqualTo(updatedVacationRequest.EndDate));
//            Assert.That(actualVacationRequest.Status, Is.EqualTo(updatedVacationRequest.Status));
//            Assert.That(actualVacationRequest.UserId, Is.EqualTo(updatedVacationRequest.User.Id));
//        }

//        [Test]
//        public async Task DeleteVacationRequest_Deletes_Existing_VacationRequest()
//        {
//            // Arrange
//            var user = new User
//            {
//                Id = Guid.NewGuid(),
//                FirstName = "John",
//                LastName = "Doe",
//                UserName = "jdoe",
//                PasswordHash = new byte[] { 1, 2, 3 },
//                PasswordSalt = new byte[] { 4, 5, 6 },
//                CountryCode = "US",
//                Role = "User",
//                StartWorkingHour = 8,
//                EndWorkingHour = 17,
//            };
//            await _context.Users.AddAsync(user);

//            var existingVacationRequest = new VacationRequest
//            {
//                requestId = Guid.NewGuid(),
//                StartDate = new DateTime(2022, 6, 1),
//                EndDate = new DateTime(2022, 6, 5),
//                Status = Status.Pending,
//                User = user
//            };
//            await _context.VacationRequests.AddAsync(existingVacationRequest);
//            await _context.SaveChangesAsync();

//            // Act
//            await _repository.DeleteVacationRequest(existingVacationRequest.requestId);

//            // Assert
//            var actualVacationRequest = await _context.VacationRequests.FindAsync(existingVacationRequest.requestId);
//            Assert.That(actualVacationRequest, Is.Null);
//        }
//    }
//}
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacationAPI.Data;
using VacationAPI.Models;
using VacationAPI.Repositories;
using VacationAPI.Services;

namespace VacationAPI.Tests.Repositories
{



    [TestFixture]
    public class UserRepositoryTests 
    {
        private ApplicationDbContext _context;
        private IUserRepository _repository;

         [SetUp]
        public async Task SetUpAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new UserRepository(_context);

            await _context.Database.EnsureCreatedAsync();
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        [Test]
        public async Task GetAll_Returns_All_Users()
        {
            // Arrange
            var expectedUsers = new List<User>
        {
            new User
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
            new User
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
        };

            await _context.Users.AddRangeAsync(expectedUsers);
            await _context.SaveChangesAsync();

            // Act
            var actualUsers = await _repository.GetAll();

            // Assert
            Assert.That(actualUsers.Count(), Is.EqualTo(expectedUsers.Count));

            foreach (var expectedUser in expectedUsers)
            {
                var actualUser = actualUsers.FirstOrDefault(u => u.Id == expectedUser.Id);
                Assert.That(actualUser, Is.Not.Null);
                Assert.That(actualUser.FirstName, Is.EqualTo(expectedUser.FirstName));
                Assert.That(actualUser.LastName, Is.EqualTo(expectedUser.LastName));
                Assert.That(actualUser.UserName, Is.EqualTo(expectedUser.UserName));
                Assert.That(actualUser.PasswordHash, Is.EqualTo(expectedUser.PasswordHash));
                Assert.That(actualUser.PasswordSalt, Is.EqualTo(expectedUser.PasswordSalt));
                Assert.That(actualUser.CountryCode, Is.EqualTo(expectedUser.CountryCode));
                Assert.That(actualUser.Role, Is.EqualTo(expectedUser.Role));
                Assert.That(actualUser.StartWorkingHour, Is.EqualTo(expectedUser.StartWorkingHour));
                Assert.That(actualUser.EndWorkingHour, Is.EqualTo(expectedUser.EndWorkingHour));
            }
        }

        [Test]
        public async Task GetById_Returns_Correct_User()
        {
            // Arrange
            var expectedUser = new User
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
            };

            await _context.Users.AddAsync(expectedUser);
            await _context.SaveChangesAsync();

            // Act
            var actualUser = await _repository.GetById(expectedUser.Id);

            // Assert
            Assert.That(actualUser, Is.Not.Null);
            Assert.That(actualUser.FirstName, Is.EqualTo(expectedUser.FirstName));
            Assert.That(actualUser.LastName, Is.EqualTo(expectedUser.LastName));
            Assert.That(actualUser.UserName, Is.EqualTo(expectedUser.UserName));
            Assert.That(actualUser.PasswordHash, Is.EqualTo(expectedUser.PasswordHash));
            Assert.That(actualUser.PasswordSalt, Is.EqualTo(expectedUser.PasswordSalt));
            Assert.That(actualUser.CountryCode, Is.EqualTo(expectedUser.CountryCode));
            Assert.That(actualUser.Role, Is.EqualTo(expectedUser.Role));
            Assert.That(actualUser.StartWorkingHour, Is.EqualTo(expectedUser.StartWorkingHour));
            Assert.That(actualUser.EndWorkingHour, Is.EqualTo(expectedUser.EndWorkingHour));
        }

        [Test]
        public async Task GetByUsername_Returns_Correct_User()
        {
            // Arrange
            var expectedUser = new User
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
            };

            await _context.Users.AddAsync(expectedUser);
            await _context.SaveChangesAsync();

            // Act
            var actualUser = await _repository.GetById(expectedUser.Id);

            // Assert
            Assert.That(actualUser, Is.Not.Null);
            Assert.That(actualUser.FirstName, Is.EqualTo(expectedUser.FirstName));
            Assert.That(actualUser.LastName, Is.EqualTo(expectedUser.LastName));
            Assert.That(actualUser.UserName, Is.EqualTo(expectedUser.UserName));
            Assert.That(actualUser.PasswordHash, Is.EqualTo(expectedUser.PasswordHash));
            Assert.That(actualUser.PasswordSalt, Is.EqualTo(expectedUser.PasswordSalt));
            Assert.That(actualUser.CountryCode, Is.EqualTo(expectedUser.CountryCode));
            Assert.That(actualUser.Role, Is.EqualTo(expectedUser.Role));
            Assert.That(actualUser.StartWorkingHour, Is.EqualTo(expectedUser.StartWorkingHour));
            Assert.That(actualUser.EndWorkingHour, Is.EqualTo(expectedUser.EndWorkingHour));
        }


        [Test]
        public async Task Get_Returns_Existing_User()
        {
            // Arrange
            var expectedUser = new User
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
            };

            await _context.Users.AddAsync(expectedUser);
            await _context.SaveChangesAsync();

            // Act
            var actualUser = await _repository.GetById(expectedUser.Id);

            // Assert
            Assert.IsNotNull(actualUser);
            Assert.AreEqual(expectedUser.FirstName, actualUser.FirstName);
            Assert.AreEqual(expectedUser.LastName, actualUser.LastName);
            Assert.AreEqual(expectedUser.UserName, actualUser.UserName);
            Assert.AreEqual(expectedUser.PasswordHash, actualUser.PasswordHash);
            Assert.AreEqual(expectedUser.PasswordSalt, actualUser.PasswordSalt);
            Assert.AreEqual(expectedUser.CountryCode, actualUser.CountryCode);
            Assert.AreEqual(expectedUser.Role, actualUser.Role);
            Assert.AreEqual(expectedUser.StartWorkingHour, actualUser.StartWorkingHour);
            Assert.AreEqual(expectedUser.EndWorkingHour, actualUser.EndWorkingHour);

        }

        [Test]
        public async Task Get_Returns_Null_For_Nonexistent_User()
        {
            // Arrange
            var nonexistentId = Guid.NewGuid();

            // Act
            var actualUser = await _repository.GetById(nonexistentId);

            // Assert
            Assert.IsNull(actualUser);

        }
    }
}
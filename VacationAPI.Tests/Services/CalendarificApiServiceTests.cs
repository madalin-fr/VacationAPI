using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using VacationAPI.Models;
using VacationAPI.Services;

namespace VacationAPI.Tests.Services
{
    [TestFixture]
    public class CalendarificApiServiceTests
    {
        [Test]
        public async Task GetHolidaysAsync_Returns_Holidays()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var httpClient = new HttpClient();
            var service = new CalendarificApiService(configuration, httpClient);

            // Act
            var result = await service.GetHolidaysAsync(2021, "US");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<NationalHoliday>>());
            Assert.That(result, Is.Not.Empty);
        }
    }
}
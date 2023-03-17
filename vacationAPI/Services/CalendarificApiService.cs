using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System;
using VacationAPI.Models;
using System.Text.Json;
using VacationAPI.Services;

namespace VacationAPI.Services
{
    public class CalendarificApiService : ICalendarificApiService
    {
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://calendarific.com/api/v2";

        public CalendarificApiService(IConfiguration configuration)
        {
            _apiKey = configuration.GetValue<string>("CalendarificApi:ApiKey");
        }

        public async Task<List<NationalHoliday>> GetHolidaysAsync(int year, string countryCode)
        {
            string url = $"{_baseUrl}/holidays?api_key={_apiKey}&country={countryCode}&year={year}&type=national";

            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                JsonDocument document = JsonDocument.Parse(json);

                JsonElement holidaysElement = document.RootElement.GetProperty("response").GetProperty("holidays");

                List<NationalHoliday> holidays = new();

                foreach (JsonElement holidayElement in holidaysElement.EnumerateArray())
                {
                    NationalHoliday nationalHoliday = new()
                    {
                        Name = holidayElement.GetProperty("name").GetString(),
                        Date = holidayElement.GetProperty("date").GetProperty("iso").GetDateTime(),
                        Description = holidayElement.GetProperty("description").GetString(),
                        CountryCode = countryCode
                    };
                    NationalHoliday holiday = nationalHoliday;

                    holidays.Add(holiday);
                }

                return holidays;
            }
            else
            {
                throw new Exception($"Failed to retrieve holidays. Status code: {response.StatusCode}");
            }
        }
    }
}
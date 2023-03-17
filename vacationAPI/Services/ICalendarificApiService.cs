using System.Collections.Generic;
using System.Threading.Tasks;
using VacationAPI.Models;

namespace VacationAPI.Services
{
    public interface ICalendarificApiService
    {
        Task<List<NationalHoliday>> GetHolidaysAsync(int year, string countryCode);
    }
}

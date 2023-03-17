using System;

namespace VacationAPI.DTOs
{
    public class NationalHolidayDTO
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string CountryCode { get; set; }
    }
}

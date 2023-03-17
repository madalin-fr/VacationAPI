using System;

namespace VacationAPI.Models
{
    public class NationalHoliday
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string CountryCode { get; set; }

    }
}

using System;

namespace VacationAPI.DTOs
{
    public class UserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string CountryCode { get; set; }
        public int AvailableVacationDaysPerYear { get; set; }
        public int StartWorkingHour { get; set; }
        public int EndWorkingHour { get; set; }

    }
}

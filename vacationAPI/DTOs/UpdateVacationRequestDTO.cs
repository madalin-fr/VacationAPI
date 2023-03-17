using System;

namespace VacationAPI.DTOs
{
    public class UpdateVacationRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Comment { get; set; }
    }
}

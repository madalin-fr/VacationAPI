using System;

namespace VacationAPI.DTOs
{
    public class CreateVacationRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Comment { get; set; }
    }
}

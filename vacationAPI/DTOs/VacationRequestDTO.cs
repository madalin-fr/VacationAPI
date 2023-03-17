using System;
using VacationAPI.Models;

namespace VacationAPI.DTOs
{
    public class VacationRequestDTO
    {
        public Guid requestId { get; set; }
        public string Username { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
    }
}
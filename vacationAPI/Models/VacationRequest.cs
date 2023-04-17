using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VacationAPI.Models;

namespace VacationAPI.Models
{
    public class VacationRequest
    {
        [Key]
        public Guid RequestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Status Status { get; set; }
        public string Comment { get; set; }
        public string Username { get; set; }
        public User User { get; set; }
        public int NumberOfDays { get; set; }

        public string GetStatusAsString()
        {
            return Status.GetType()
                .GetMember(Status.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .OfType<DescriptionAttribute>()
                .FirstOrDefault()?.Description;
        }

        public VacationRequest()
        {
            Comment = "";
        }

    }
}


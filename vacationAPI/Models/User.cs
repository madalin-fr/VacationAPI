using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace VacationAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string CountryCode { get; set; }
        public string Role { get; set; }

        public int StartWorkingHour { get; set; }
        public int EndWorkingHour { get; set; }

        // List useful for calculating vacation days used
        public List<VacationRequest> VacationRequests { get; set; }

        public int AvailableVacationDaysPerYear { get; set; } = 25;


        public User(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserName = user.UserName;
            PasswordHash = user.PasswordHash;
            PasswordSalt = user.PasswordSalt;
            CountryCode = user.CountryCode;
            Role = user.Role;
            StartWorkingHour = user.StartWorkingHour;
            EndWorkingHour = user.EndWorkingHour;
            VacationRequests = user.VacationRequests;
            AvailableVacationDaysPerYear = user.AvailableVacationDaysPerYear;
        }

        public User()
        {

        }
    }
}
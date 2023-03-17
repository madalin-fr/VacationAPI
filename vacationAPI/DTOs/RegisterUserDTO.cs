using System.ComponentModel.DataAnnotations;

namespace VacationAPI.DTOs
{
    public class RegisterUserDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(2)]
        public string CountryCode { get; set; }

        [Required]
        [RegularExpression("(?i)(Admin|User)")] // any cases
        public string Role { get; set; }

        [Required]
        [Range(0, 23)]
        public int StartWorkingHour { get; set; }

        [Required]
        [Range(0, 23)]
        public int EndWorkingHour { get; set; }
    }
}
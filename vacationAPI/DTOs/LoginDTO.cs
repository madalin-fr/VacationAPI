using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace VacationAPI.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

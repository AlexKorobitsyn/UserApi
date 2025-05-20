//Models.DTO.UserUpdateDto
using System.ComponentModel.DataAnnotations;

namespace UserApi.Models.DTO
{
    public class UserUpdateDto
    {
        public string? Name { get; set; }
        public int? Gender { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
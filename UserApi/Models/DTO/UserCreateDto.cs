//Models.DTO.UserCreateDto
using System.ComponentModel.DataAnnotations;

namespace UserApi.Models.DTO
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Логин обязателен")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        public string Name { get; set; }

        [Range(0, 2, ErrorMessage = "Пол должен быть 0, 1 или 2")]
        public int Gender { get; set; }

        public DateTime? Birthday { get; set; }
        public bool IsAdmin { get; set; }
    }
}
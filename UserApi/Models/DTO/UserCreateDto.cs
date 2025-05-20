//Models.DTO.UserCreateDto
using System.ComponentModel.DataAnnotations;

namespace UserApi.Models.DTO
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "����� ����������")]
        public string Login { get; set; }

        [Required(ErrorMessage = "������ ����������")]
        public string Password { get; set; }

        [Required(ErrorMessage = "��� �����������")]
        public string Name { get; set; }

        [Range(0, 2, ErrorMessage = "��� ������ ���� 0, 1 ��� 2")]
        public int Gender { get; set; }

        public DateTime? Birthday { get; set; }
        public bool IsAdmin { get; set; }
    }
}
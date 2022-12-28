using System.ComponentModel.DataAnnotations;

namespace twitter_copy.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Не введен пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

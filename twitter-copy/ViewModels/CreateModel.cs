using System;
using System.ComponentModel.DataAnnotations;

namespace twitter_copy.ViewModels
{
    public class CreateModel
    {
        [Required(ErrorMessage = "Вы не ввели контент")]
        public string postText { get; set; }
        [Required(ErrorMessage = "Вы не указали дату")]
        public DateTime Date_of_creating { get; set; }
        [Required(ErrorMessage = "Вы не авторизованы")]
        public string UserName { get; set; }
    }
}

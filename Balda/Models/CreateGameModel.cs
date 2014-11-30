using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Balda.Models
{
    public class CreateGameModel
    {
        [Required]
        [DisplayName("Ваше имя")]
        public string Player1 { get; set; }
        [Required]
        [DisplayName("Уникальное имя игры")]
        public string GameId { get; set; }
        [Required]
        [DisplayName("Пароль на игру")]
        public string Password { get; set; }
    }
}
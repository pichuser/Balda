using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Balda.Models
{
    public class EnterToGameModel
    {
        [Required]
        [DisplayName("Уникальное имя игры")]
        public String GameId { get; set; }
        [Required]
        [DisplayName("Пароль")]
        public String GamePassword { get; set; }
        [Required]
        [DisplayName("Ваше имя")]
        public String UserName { get; set; }
    }
}
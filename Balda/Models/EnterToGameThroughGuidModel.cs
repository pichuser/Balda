using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Balda.Models
{
    public class EnterToGameThroughGuidModel
    {
        [Required]
        public Guid GameGuid { get; set; }
        [Required]
        [DisplayName("Введите ваше имя")]
        public string UserName { get; set; }
    }
}
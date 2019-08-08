using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ArenaOIA.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingrese usuario")]
        [DisplayName("Usuario")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Ingrese la contraseña")]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña")]
        public string Password { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace webVentaLibros.Models
{
    public class UserModel
    {
        [Required(ErrorMessage = "Usuario requerido")]
        [Display(Name = "Usuario")]
        public string usuario { get; set; }

        [Required(ErrorMessage = "Contraseña requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string contraseña { get; set; }

        public string nombre { get; set; }
        public string direccion { get; set; }
        public int idUsuario { get; set; }
    }
}
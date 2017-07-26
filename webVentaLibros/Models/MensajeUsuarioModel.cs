using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class MensajeUsuarioModel
    {
        public int idMensaje { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public string mensaje { get; set; }
        public int idTipoMensaje { get; set; }
        public string tipoMensaje { get; set; }
        public DateTime fechaHora { get; set; }
    }
}
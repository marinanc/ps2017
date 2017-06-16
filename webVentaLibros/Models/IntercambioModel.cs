using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class IntercambioModel
    {
        public int idPublicacionUsuario1 { get; set; }
        public int idPublicacionUsuario2 { get; set; }
        public DateTime fechaHora { get; set; }
        public string comentario { get; set; }
        public int calificacionUsuario1 { get; set; }
        public int calificacionUsuario2 { get; set; }
        public int idEstado { get; set; }

    }
}
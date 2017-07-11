using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class CalificacionPorLibroModel
    {
        public string codigoLibro { get; set; }
        public int idUsuario { get; set; }
        public int calificacion { get; set; }
        public DateTime fechaHora { get; set; }
        public string comentario { get; set; }
    }
}
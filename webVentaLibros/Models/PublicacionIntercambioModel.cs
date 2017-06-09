using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class PublicacionIntercambioModel
    {
        public int idPublicacion { get; set; }
        public int idUsuario { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string foto { get; set; }
        public int idEstado { get; set; }
        public int idGenero { get; set; }
        public string genero { get; set; }
        public string autor { get; set; }
        public DateTime fechaHoraAlta { get; set; }
    }
}
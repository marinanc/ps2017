using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class DescuentosModel
    {
        public int idDescuento { get; set; }
        public string codigo { get; set; }
        public int idTipo { get; set; }
        public DateTime fecha { get; set; }
        public int validez { get; set; }
        public DateTime fechaExpiracion { get; set; }
        public int descuento { get; set; }
    }
}
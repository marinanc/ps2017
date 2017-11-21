using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class ReclamoModel
    {
        public int idReclamo { get; set; }
        public int idPedido { get; set; }
        public string email { get; set; }
        public string mensaje { get; set; }
        public DateTime fecha { get; set; }
    }
}
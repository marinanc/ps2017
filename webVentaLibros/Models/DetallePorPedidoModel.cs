using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class DetallePorPedidoModel
    {
        public int idPedido { get; set; }
        public string codigoLibro { get; set; }
        public int cantidad { get; set; }
        public double precioUnitario { get; set; }
    }
}
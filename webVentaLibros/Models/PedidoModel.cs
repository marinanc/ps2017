using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class PedidoModel
    {
        public int idPedido { get; set; }
        public int idUsuario { get; set; }
        public DateTime fechaHora { get; set; }
        public int idEstado { get; set; }
        public string estado { get; set; }
        public List<DetallePorPedido> detalles { get; set; }
    }
}
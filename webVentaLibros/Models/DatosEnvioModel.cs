using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class DatosEnvioModel
    {
        public string nombreUsuario { get; set; }
        public string apellidoUsuario { get; set; }
        public int idProvincia { get; set; }
        public string provincia { get; set; }
        public int idLocalidad { get; set; }
        public string localidad { get; set; }
        public string codigoPostal { get; set; }
        public string mail { get; set; }
        public string direccion { get; set; }
    }
}
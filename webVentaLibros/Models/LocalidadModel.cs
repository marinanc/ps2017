using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class LocalidadModel
    {
        public int idLocalidad { get; set; }
        public int idProvincia { get; set; }
        public string nombreLocalidad { get; set; }
    }
}
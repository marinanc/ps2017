using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class ProvinciaModel
    {
        public int idProvincia { get; set; }
        public string nombreProvincia { get; set; }
        public List<LocalidadModel> localidades { get; set; }

    }
}
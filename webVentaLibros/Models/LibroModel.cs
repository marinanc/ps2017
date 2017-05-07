using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class LibroModel
    {
        public string codigoBarra { get; set; }
        public string titulo { get; set; }
        public int idEditorial { get; set; }
        public int paginas { get; set; }
        public int edicion { get; set; }
        public int idGenero { get; set; }
        public string foto { get; set; }
        public string sinopsis { get; set; }
        public double precio { get; set; }
        public int stock { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webVentaLibros.Models
{
    public class CarritoItem
    {
        private LibroModel libro;

        public LibroModel Libro
        {
            get { return libro; }
            set { libro = value; }
        }
        private int cantidad;

        public int Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }

        public CarritoItem()
        {

        }
        public CarritoItem(LibroModel libro, int cantidad)
        {
            this.libro = libro;
            this.cantidad = cantidad;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class CartController : Controller
    {
        //
        // GET: /Cart/
        [HttpGet]
        public ActionResult Carrito()
        {
            double totalCompra = 0;
            if (Session["carrito"] != null)
            {
                foreach (var item in Session["carrito"] as List<CarritoItem>)
                {
                    totalCompra = totalCompra + (item.Cantidad * item.Libro.precio);
                }
                ViewBag.total = totalCompra;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Carrito(string codLibro, int quantity)
        {
            double totalCompra = 0;
            var bd = new bdVentaLibrosDataContext();
            var libroAgregar = from libro in bd.Libros
                               where libro.codigoBarra == codLibro
                               select new LibroModel
                               {
                                     codigoBarra = libro.codigoBarra,
                                     foto = libro.foto,
                                     titulo = libro.titulo, 
                                     precio = Convert.ToDouble(libro.precio)
                                };
            var lista = libroAgregar.ToList();

            if (Session["carrito"] == null)
            {
                List<CarritoItem> compra = new List<CarritoItem>();
                compra.Add(new CarritoItem(lista.First(), quantity));
                Session["carrito"] = compra;
            }
            else
            {
                List<CarritoItem> compra = (List<CarritoItem>)Session["carrito"];
                int indexExistente = getIndex(codLibro);
                if (indexExistente == -1)
                    compra.Add(new CarritoItem(lista.First(), quantity));
                else
                    compra[indexExistente].Cantidad++;
                Session["carrito"] = compra;
            }
            foreach(var item in Session["carrito"] as List<CarritoItem>){
                totalCompra = totalCompra + (item.Cantidad * item.Libro.precio);
            }
            ViewBag.total = totalCompra;
            return View();
        }

        public ActionResult Eliminar(string codLibro)
        {
            double totalCompra = 0;
            List<CarritoItem> compra = (List<CarritoItem>)Session["carrito"];
            compra.RemoveAt(getIndex(codLibro));
            foreach (var item in Session["carrito"] as List<CarritoItem>)
            {
                totalCompra = totalCompra + (item.Cantidad * item.Libro.precio);
            }
            ViewBag.total = totalCompra;
            return View("Carrito");

        }

        public ActionResult Checkout()
        {
            double totalCompra = 0;
            if (Session["carrito"] != null)
            {
                foreach (var item in Session["carrito"] as List<CarritoItem>)
                {
                    totalCompra = totalCompra + (item.Cantidad * item.Libro.precio);
                }
                ViewBag.total = totalCompra;
            }
            return View();
        }

        private int getIndex(string codLibro)
        {
            List<CarritoItem> compra = (List<CarritoItem>)Session["carrito"];
            for (int i = 0; i < compra.Count; i++)
            {
                if (compra[i].Libro.codigoBarra == codLibro)
                    return i;
            }
            return -1;
        }
    }
}

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
            return View();
        }

        [HttpPost]
        public ActionResult Carrito(string codLibro, int quantity)
        {
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
                compra.Add(new CarritoItem(lista.First(), quantity));
                Session["carrito"] = compra;
            }

            return View();
        }

    }
}

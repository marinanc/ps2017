using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class BusquedaController : Controller
    {
        //
        // GET: /Busqueda/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscarLibro(string busqueda)
        {
            var bd = new bdVentaLibrosDataContext();

            var librosEncontrados = (from libro in bd.Libros
                                    where libro.codigoBarra == busqueda || libro.titulo.Contains(busqueda)
                                    select libro).ToList();

            var listaCompleta = new List<LibroModel>();

            foreach (var libro in librosEncontrados)
            {
                var entidadLibro = new LibroModel
                {
                    codigoBarra = libro.codigoBarra,
                    foto = libro.foto,
                    titulo = libro.titulo,
                    sinopsis = libro.sinopsis,
                    precio = Convert.ToDouble(libro.precio)
                };
                listaCompleta.Add(entidadLibro);
            }

            return View(listaCompleta);
        }
    }
}

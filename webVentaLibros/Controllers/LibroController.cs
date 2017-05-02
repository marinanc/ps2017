using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class LibroController : Controller
    {
        //
        // GET: /Libro/

        public ActionResult Index(string cod)
        {
            var bdVentaLibros = new bdVentaLibrosDataContext();

            var libroSeleccionado = from libro in bdVentaLibros.Libros
                                    where libro.codigoBarra == cod
                                    select new LibroModel
                                    {
                                        codigoBarra = libro.codigoBarra,
                                        titulo = libro.titulo,
                                        edicion = Convert.ToInt32(libro.edicion),
                                        foto = libro.foto,
                                        idGenero = Convert.ToInt32(libro.idGenero),
                                        idEditorial = Convert.ToInt32(libro.idEditorial),
                                        sinopsis = libro.sinopsis,
                                        precio = Convert.ToDouble(libro.precio),
                                        paginas = Convert.ToInt32(libro.paginas)
                                    };
            
            return View(libroSeleccionado);
        }

        

    }
}

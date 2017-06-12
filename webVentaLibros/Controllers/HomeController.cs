using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            //Creo el contexto de datos 
            var dbLibros = new bdVentaLibrosDataContext();
            //Recupero la lista de libros -> son de tipo Libro: entidad de dominio

            var lista = (from libro in dbLibros.Libros
                         select new LibroModel
                         {
                             codigoBarra = libro.codigoBarra,
                             foto = libro.foto,
                             titulo = libro.titulo, 
                             sinopsis = libro.sinopsis,
                             precio = Convert.ToDouble(libro.precio),
                             fechaAlta = Convert.ToDateTime(libro.fechaAlta)
                         }).OrderByDescending(l => l.fechaAlta).Take(10).ToList();

            var listaCompleta = new List<LibroModel>();

            foreach (var libro in lista)
            {
                var entidadLibro = new LibroModel
                {
                    codigoBarra = libro.codigoBarra,
                    foto = libro.foto,
                    titulo = libro.titulo,
                    sinopsis = libro.sinopsis,
                    precio = libro.precio
                };
                listaCompleta.Add(entidadLibro);
            }
           
            //Paso como parámetro a la vista la lista de inmuebles
            return View(listaCompleta);

        }

    }
}

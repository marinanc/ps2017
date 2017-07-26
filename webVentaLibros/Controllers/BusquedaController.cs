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
                                    where libro.codigoBarra == busqueda || 
                                    libro.titulo.Contains(busqueda) ||
                                    (libro.Autores.nombres+" "+libro.Autores.apellidos).Contains(busqueda) ||
                                    (libro.Autores1.nombres + " " + libro.Autores1.apellidos).Contains(busqueda) ||
                                    (libro.Autores2.nombres + " " + libro.Autores2.apellidos).Contains(busqueda) ||
                                    (libro.Autores3.nombres + " " + libro.Autores3.apellidos).Contains(busqueda)
                                    select libro).ToList();

            ViewBag.listadoLibros = librosEncontrados;

            var listaGeneros = (from genero in bd.Generos
                                select new GeneroModel
                                {
                                    idGenero = genero.idGenero,
                                    nombreGenero = genero.nombre
                                }).ToList();

            ViewBag.listadoGeneros = listaGeneros;

            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class CatalogoController : Controller
    {
        //
        // GET: /Catalogo/

        public ActionResult Catalogo()
        {
            var bd = new bdVentaLibrosDataContext();

            var listaGeneros = (from genero in bd.Generos
                                select new GeneroModel
                                {
                                    idGenero = genero.idGenero,
                                    nombreGenero = genero.nombre
                                }).ToList();
            ViewBag.listadoGeneros = listaGeneros;
            return View();
        }


        public ActionResult MostrarLibros(int idGenero)
        {
            var bd = new bdVentaLibrosDataContext();

            var listaLibros = (from libro in bd.Libros
                               where libro.idGenero == idGenero
                               select new LibroModel
                               {
                                   codigoBarra = libro.codigoBarra,
                                   titulo = libro.titulo,
                                   foto = libro.foto,
                                   precio = Convert.ToDouble(libro.precio)
                               }).ToList();

            var listaGeneros = (from genero in bd.Generos
                                select new GeneroModel
                                {
                                    idGenero = genero.idGenero,
                                    nombreGenero = genero.nombre
                                }).ToList();

            ViewBag.listadoGeneros = listaGeneros;
            ViewBag.listadoPublicaciones = listaLibros;

            return View();
        }
    }
}

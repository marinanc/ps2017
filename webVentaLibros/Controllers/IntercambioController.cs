using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class IntercambioController : Controller
    {
        //
        // GET: /Intercambio/

        public ActionResult Intercambios()
        {
            var bd = new bdVentaLibrosDataContext();

            var listaGeneros = (from genero in bd.Generos
                                select new GeneroModel
                                {
                                    idGenero = genero.idGenero,
                                    nombreGenero = genero.nombre
                                }).ToList();
            ViewBag.listadoGeneros = listaGeneros;

            var listaPublicaciones = from libro in bd.PublicacionIntercambio
                                     where libro.idEstado == 1
                                     select libro;

            ViewBag.listadoPublicaciones = listaPublicaciones;

            return View();
        }

        public ActionResult BusquedaLibro(string busqueda)
        {
            var bd = new bdVentaLibrosDataContext();

            ViewBag.listadoGeneros = from genero in bd.Generos
                               select genero;

            ViewBag.librosEncontrados = (from libro in bd.PublicacionIntercambio
                                     where libro.autor.Contains(busqueda) || libro.titulo.Contains(busqueda)
                                     where libro.idEstado == 1
                                     select libro).ToList();

            return View();
        }

        public ActionResult MostrarLibros(int idGenero)
        {
            var bd = new bdVentaLibrosDataContext();

            var listaLibros = from libro in bd.PublicacionIntercambio
                              where libro.idGenero == idGenero
                              where libro.idEstado == 1
                              select libro;
                               

            var listaGeneros = from genero in bd.Generos
                                select genero;

            ViewBag.listadoGeneros = listaGeneros;
            ViewBag.listadoLibros = listaLibros;

            return View();
        }

        public ActionResult DetalleLibro(int idPublicacion)
        {
            var bd = new bdVentaLibrosDataContext();
            
            ViewBag.listadoGeneros = from genero in bd.Generos
                               select genero;

            ViewBag.listadoLibros = from libro in bd.PublicacionIntercambio
                                    where libro.idPublicacion == idPublicacion
                                    where libro.idEstado == 1
                                    from genero in bd.Generos
                                    where libro.idGenero == genero.idGenero
                                    select new PublicacionIntercambioModel { 
                                        idPublicacion = libro.idPublicacion,
                                        titulo = libro.titulo,
                                        foto = libro.foto,
                                        genero = genero.nombre,
                                        descripcion = libro.descripcion,
                                        autor = libro.autor,
                                        fechaHoraAlta = Convert.ToDateTime(libro.fechaHoraAlta)
                                    };

            return View();
        }
    }
}
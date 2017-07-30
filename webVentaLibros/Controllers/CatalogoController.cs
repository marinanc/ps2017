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

            ViewBag.novedades = (from libro in bd.Libros
                                 where libro.stock > 0
                                 orderby libro.fechaAlta descending
                                 select libro).Take(20);

            return View();
        }


        public ActionResult MostrarLibros(int idGenero)
        {
            var bd = new bdVentaLibrosDataContext();

            var listaLibros = (from libro in bd.Libros
                               where libro.idGenero == idGenero
                               && libro.stock > 0
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

            ViewBag.nombreGenero = (from genero in bd.Generos
                                    where genero.idGenero == idGenero
                                    select genero.nombre).FirstOrDefault();

            ViewBag.listadoGeneros = listaGeneros;
            ViewBag.listadoLibros = listaLibros;

            return View();
        }

        public ActionResult LoMasVendido()
        {
            var bd = new bdVentaLibrosDataContext();

            ViewBag.loMasVendido = (from libro in bd.DetallePorPedido
                                    from libroExistencia in bd.Libros
                                    where libro.codigoLibro == libroExistencia.codigoBarra
                                    && libroExistencia.stock > 0
                                    group libro by libro.codigoLibro into g
                                    orderby g.Sum(x => x.cantidad) descending
                                    select g.First()).Take(10).ToList();

            ViewBag.listadoGeneros = (from genero in bd.Generos
                                    select new GeneroModel
                                    {
                                        idGenero = genero.idGenero,
                                        nombreGenero = genero.nombre
                                    }).ToList();

            return View();
        }

        public ActionResult MejoresCalificados()
        {
            var bd = new bdVentaLibrosDataContext();

            ViewBag.mejorCalificacion = (from libro in bd.CalificacionPorLibro
                                         from libroExistencia in bd.Libros
                                         where libro.codigoLibro == libroExistencia.codigoBarra
                                         && libroExistencia.stock > 0
                                         group libro by libro.codigoLibro into g
                                         orderby g.Average(x => x.calificacion) descending
                                         select g.First()).Take(10).ToList();

            ViewBag.listadoGeneros = (from genero in bd.Generos
                                    select new GeneroModel
                                    {
                                        idGenero = genero.idGenero,
                                        nombreGenero = genero.nombre
                                    }).ToList();

            return View();
        }

        public ActionResult LoMasDeseado()
        {
            var bd = new bdVentaLibrosDataContext();

            ViewBag.loMasDeseado = (from libro in bd.ListaDeseados
                                    from libroExistencia in bd.Libros
                                    where libro.codigoLibro == libroExistencia.codigoBarra
                                    && libroExistencia.stock > 0
                                    group libro by libro.codigoLibro into g
                                    orderby g.Count() descending
                                    select g.First()).Take(10).ToList();

            ViewBag.listadoGeneros = (from genero in bd.Generos
                                    select new GeneroModel
                                    {
                                        idGenero = genero.idGenero,
                                        nombreGenero = genero.nombre
                                    }).ToList();

            return View();
        }
    }
}

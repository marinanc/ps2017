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

            int idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);

            ViewBag.listadoGeneros = from genero in bd.Generos
                                     select genero;
                               

            ViewBag.listadoPublicaciones = from libro in bd.PublicacionIntercambio
                                     where libro.idEstado == 1
                                     where libro.idUsuario != idUsuario
                                     select libro;

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
            ViewBag.listadoPublicaciones = listaLibros;

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
                                    select libro;

            //Para listar los libros del usuario conectado
            int idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);
            ViewBag.listadoLibrosPublicados = from publicacion in bd.PublicacionIntercambio
                                              where publicacion.idUsuario == idUsuario
                                              from genero in bd.Generos
                                              where publicacion.idGenero == genero.idGenero
                                              select new PublicacionIntercambioModel
                                              {
                                                  idPublicacion = publicacion.idPublicacion,
                                                  titulo = publicacion.titulo,
                                                  foto = publicacion.foto,
                                                  autor = publicacion.autor,
                                                  genero = genero.nombre,
                                                  fechaHoraAlta = Convert.ToDateTime(publicacion.fechaHoraAlta)
                                              };

            return View();
        }

        [HttpPost]
        public ActionResult PedirIntercambio(int codPublicacionUsu1, int codPublicacionUsu2)
        {
            var bd = new bdVentaLibrosDataContext();

            if (!intercambioExiste(codPublicacionUsu1, codPublicacionUsu2))
            {
                Intercambios intercambio = new Intercambios
                {
                    idPublicacionUsuario1 = codPublicacionUsu1,
                    idPublicacionUsuario2 = codPublicacionUsu2,
                    fechaHora = DateTime.Now,
                    idEstado = 1
                };

                bd.Intercambios.InsertOnSubmit(intercambio);
                bd.SubmitChanges();
                TempData["Message"] = "¡Se ha enviado tu solicitud de intercambio!";
            }
            else
            {
                TempData["Message"] = "Ya has enviado una solicitud igual";
            }

            return RedirectToAction("Intercambios");
        }

        private bool intercambioExiste(int cod1, int cod2)
        {
            bool existe = false;
            using (var bd = new bdVentaLibrosDataContext())
            {
                var publicacion1 = bd.Intercambios.FirstOrDefault(i => i.idPublicacionUsuario1 == cod1);
                var publicacion2 = bd.Intercambios.FirstOrDefault(i => i.idPublicacionUsuario2 == cod2);
                if (publicacion1 != null && publicacion2 != null)
                {
                    existe = true;
                }
            }
            return existe;
        }
    }
}
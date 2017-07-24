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

            int idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);

            ViewBag.listadoGeneros = from genero in bdVentaLibros.Generos
                                     select genero;

            var libroSeleccionado = from libro in bdVentaLibros.Libros
                                    join gen in bdVentaLibros.Generos on libro.idGenero equals gen.idGenero
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
                                        paginas = Convert.ToInt32(libro.paginas),
                                        genero = gen.nombre
                                    };

            var calificacionPromedio = (from calificacion in bdVentaLibros.CalificacionPorLibro
                                        where calificacion.codigoLibro == cod
                                        select calificacion.calificacion).ToList();

            if (calificacionPromedio.Count > 0)
            {
                ViewBag.calificacionPromedio = Convert.ToInt32(calificacionPromedio.Average());
            }

            ViewBag.enListaDeseados = (from deseado in bdVentaLibros.ListaDeseados
                                       where deseado.idUsuario == idUsuario
                                       && deseado.codigoLibro == cod
                                       select deseado).Count();

            //libros relacionados (genero, autor...)
            var libroElegido = (from libro in bdVentaLibros.Libros
                                where libro.codigoBarra == cod
                                select libro).FirstOrDefault();

            ViewBag.librosRelacionados = (from libro in bdVentaLibros.Libros
                                          where (libro.idGenero == libroElegido.idGenero
                                          || libro.idAutor1 == libroElegido.idAutor1)
                                          && libro.codigoBarra != libroElegido.codigoBarra
                                          select libro).Take(7);

            return View(libroSeleccionado);
        }

        public ActionResult RegistrarCalificacion(string codLibro, int rating, string review)
        {
            var bd = new bdVentaLibrosDataContext();

            int idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);

            try
            {
                if (verificarCalificacion(codLibro, idUsuario)) //Ya realizo una calificacion al libro...entonces hago update
                {
                    var updateCalificacion = from calificacion in bd.CalificacionPorLibro
                                             where calificacion.codigoLibro == codLibro &&
                                             calificacion.idUsuario == idUsuario
                                             select calificacion;

                    foreach (var calificacionLibro in updateCalificacion)
                    {
                        calificacionLibro.calificacion = rating;
                        calificacionLibro.fechaHora = DateTime.Now;
                        calificacionLibro.comentario = review;
                    }
                }
                else //No realizo una calificacion al libro... entonces hago un insert
                {
                    CalificacionPorLibro calificacion = new CalificacionPorLibro
                    {
                        codigoLibro = codLibro,
                        idUsuario = idUsuario,
                        calificacion = rating,
                        fechaHora = DateTime.Now,
                        comentario = review
                    };

                    bd.CalificacionPorLibro.InsertOnSubmit(calificacion);
                }

                bd.SubmitChanges();
                TempData["Message"] = "Calificación exitosa";
            }
            catch (Exception e)
            {
                TempData["Message"] = "No se pudo registrar la calificacion. Intentelo nuevamente." + e.Message;
            }

            return RedirectToAction("Index", new { codigo = codLibro });
        }

        public bool verificarCalificacion(string codLibro, int usuario)
        {
            var existe = false;
            var bd = new bdVentaLibrosDataContext();

            //Hago un listado de las calificaciones del libro en cuestion
            var calificacionesLibro = from libro in bd.CalificacionPorLibro
                                      where libro.codigoLibro == codLibro
                                      select libro;

            //Compruebo de que el libro tenga alguna calificacion
            if (calificacionesLibro != null)
            {
                foreach (var libro in calificacionesLibro)
                {
                    if (libro.idUsuario == usuario) // por cada calificacion que recibio, compruebo si el usuario ya realizo una calificacion
                    {
                        existe = true; //el usuario ya realizo una calificacion a ese libro..
                        break; //dejo de buscar..
                    }
                }
            }

            return existe;
        }

        public ActionResult CheckLibroDeseado(bool check, string cod)
        {
            var bd = new bdVentaLibrosDataContext();

            int idUsuarioLogueado = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);

            if (!check) //Quitar de la lista de deseados..
            {
                var libroSeleccionado = from libro in bd.ListaDeseados
                                        where libro.idUsuario == idUsuarioLogueado
                                        && libro.codigoLibro == cod
                                        select libro;

                foreach (var libro in libroSeleccionado)
                {
                    bd.ListaDeseados.DeleteOnSubmit(libro);
                }
                TempData["Message"] = "Libro eliminado de su lista de deseados";
            }
            else //Agregar a la lista de deseados..
            {
                ListaDeseados nuevoDeseado = new ListaDeseados
                {
                    idUsuario = idUsuarioLogueado,
                    codigoLibro = cod,
                    fechaHora = DateTime.Now
                };

                bd.ListaDeseados.InsertOnSubmit(nuevoDeseado);
                TempData["Message"] = "Libro agregado a su lista de deseados";
            }

            try
            { 
                bd.SubmitChanges();
            }
            catch (Exception ex)
            {
                TempData["Message"] = "No se pudo registrar. Intentelo nuevamente." + ex.Message;
            }

            return RedirectToAction("Index", new { cod = cod });
        }
    }
}

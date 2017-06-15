using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class CuentaUsuarioController : Controller
    {
        //
        // GET: /CuentaUsuario/

        public ActionResult MiCuenta()
        {
            return View();
        }

        [HttpGet]
        public ActionResult MisIntercambios()
        {
            var bd = new bdVentaLibrosDataContext();

            int idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);

            var usuarioLogueado = (from usuario in bd.Usuarios
                                  where usuario.idUsuario == idUsuario
                                  select usuario).FirstOrDefault();

            ViewBag.listadoLibrosPublicados = from publicacion in bd.PublicacionIntercambio
                                              where publicacion.idUsuario == idUsuario
                                              from genero in bd.Generos
                                              where publicacion.idGenero == genero.idGenero
                                              select new PublicacionIntercambioModel { 
                                                  idPublicacion = publicacion.idPublicacion,
                                                  titulo = publicacion.titulo,
                                                  foto = publicacion.foto,   
                                                  autor = publicacion.autor,
                                                  genero = genero.nombre,
                                                  fechaHoraAlta = Convert.ToDateTime(publicacion.fechaHoraAlta)
                                              };

            ViewBag.listadoGeneros = from genero in bd.Generos
                                select genero;

            return View();
        }

        [HttpPost]
        public ActionResult NuevoIntercambio(PublicacionIntercambioModel ppum, HttpPostedFileBase foto)
        {
            var bd = new bdVentaLibrosDataContext();

            if (foto != null) 
            { 
                foto.SaveAs(System.IO.Path.Combine(@"D:\webVentaLibros\webVentaLibros\img\catalogoIntercambios", System.IO.Path.GetFileName(foto.FileName)));
            }

            PublicacionIntercambio publicacion = new PublicacionIntercambio
            {
                idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]),
                titulo = ppum.titulo,
                descripcion = ppum.descripcion,
                foto = @"img/catalogoIntercambios/" + foto.FileName,
                idEstado = 1,
                idGenero = ppum.idGenero,
                autor = ppum.autor, 
                fechaHoraAlta = DateTime.Now

            };

            bd.PublicacionIntercambio.InsertOnSubmit(publicacion);
            bd.SubmitChanges();
            TempData["Message"] = "Se ha publicado su libro!";

            return RedirectToAction("MisIntercambios");
        }

        [HttpGet]
        public ActionResult ModificarIntercambio(int idPublicacion)
        {

            var bd = new bdVentaLibrosDataContext();

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

            ViewBag.listadoGeneros = from genero in bd.Generos
                                     select genero;

            var publicacionModificar = from publicacion in bd.PublicacionIntercambio
                                       where idPublicacion == publicacion.idPublicacion
                                       select publicacion;
                               
            return View(publicacionModificar);
        }

        [HttpPost]
        public ActionResult ModificarIntercambio(PublicacionIntercambioModel publicacionModificar, HttpPostedFileBase foto)
        {
            var bd = new bdVentaLibrosDataContext();

            if (foto != null)
            {
                foto.SaveAs(System.IO.Path.Combine(@"D:\webVentaLibros\webVentaLibros\img\catalogoIntercambios", System.IO.Path.GetFileName(foto.FileName)));
            }

            var publicacionModificada = from publicacion in bd.PublicacionIntercambio
                                  where publicacionModificar.idPublicacion == publicacion.idPublicacion
                                  select publicacion;

            //Cambio los datos del inmueble
            if (foto != null)
            {
                foreach (var publicacion in publicacionModificada)
                {
                    publicacion.titulo = publicacionModificar.titulo;
                    publicacion.idGenero = publicacionModificar.idGenero;
                    publicacion.autor = publicacionModificar.autor;
                    publicacion.descripcion = publicacionModificar.descripcion;
                    publicacion.foto = @"img/catalogoIntercambios/" + foto.FileName;

                }
            }
            else
            {
                foreach (var publicacion in publicacionModificada)
                {
                    publicacion.titulo = publicacionModificar.titulo;
                    publicacion.idGenero = publicacionModificar.idGenero;
                    publicacion.autor = publicacionModificar.autor;
                    publicacion.descripcion = publicacionModificar.descripcion;
                }
            }
            //Hago el submit
            bd.SubmitChanges();
            TempData["Message"] = "¡Publicacion de intercambio modificada!";
            return RedirectToAction("MisIntercambios");
        }

    }
}

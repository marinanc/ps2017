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
            int idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);

            var bd = new bdVentaLibrosDataContext();

            ViewBag.datosUsuario = from usuario in bd.Usuarios
                                   where usuario.idUsuario == idUsuario
                                   select usuario;

            ViewBag.totalComprasConcretadas = (from pedido in bd.Pedidos
                                    where pedido.idUsuario == idUsuario
                                    && pedido.idEstadoPedido == 5
                                    select pedido).Count();

            ViewBag.totalIntercambiosConcretados = (from intercambio in bd.Intercambios
                                                    where intercambio.idEstado == 3
                                                    && (intercambio.PublicacionIntercambio.Usuarios.idUsuario == idUsuario ||
                                                    intercambio.PublicacionIntercambio1.Usuarios.idUsuario == idUsuario)
                                                    select intercambio).Count();

            ViewBag.cantidadPedidosIntercambioRecibidos = (from publicacion in bd.PublicacionIntercambio
                                                           where publicacion.idUsuario == idUsuario
                                                           from intercambio in bd.Intercambios
                                                           where publicacion.idPublicacion == intercambio.idPublicacionUsuario1
                                                           select intercambio).Count();

            return View();
        }

        [HttpGet]
        public ActionResult MisPublicacionesActivas()
        {
            var bd = new bdVentaLibrosDataContext();

            int idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);

            var usuarioLogueado = (from usuario in bd.Usuarios
                                   where usuario.idUsuario == idUsuario
                                   select usuario).FirstOrDefault();

            ViewBag.listadoLibrosPublicados = from publicacion in bd.PublicacionIntercambio
                                              where publicacion.idUsuario == idUsuario &&
                                              publicacion.idEstado == 1
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

            return RedirectToAction("MisPublicacionesActivas");
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
            return RedirectToAction("MisPublicacionesActivas");
        }

        public ActionResult SolicitudesIntercambio()
        {
            var bd = new bdVentaLibrosDataContext();
            int idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);

            ViewBag.solicitudesIntercambioRecibidos = from publicacion in bd.PublicacionIntercambio
                                                      where publicacion.idUsuario == idUsuario
                                                      from intercambio in bd.Intercambios
                                                      where publicacion.idPublicacion == intercambio.idPublicacionUsuario1
                                                      && intercambio.idEstado == 1
                                                      select intercambio;

            ViewBag.solicitudesIntercambioEnviados = from publicacion in bd.PublicacionIntercambio
                                                     where publicacion.idUsuario == idUsuario
                                                     from intercambio in bd.Intercambios
                                                     where publicacion.idPublicacion == intercambio.idPublicacionUsuario2
                                                     && intercambio.idEstado == 1
                                                     select intercambio;

            ViewBag.solicitudesAceptadas = from publicacion in bd.PublicacionIntercambio
                                           where publicacion.idUsuario == idUsuario
                                           from intercambio in bd.Intercambios
                                           where (publicacion.idPublicacion == intercambio.idPublicacionUsuario2 
                                           || publicacion.idPublicacion == intercambio.idPublicacionUsuario1)
                                           && intercambio.idEstado == 2
                                           select intercambio;

            ViewBag.intercambiosRealizados = from publicacion in bd.PublicacionIntercambio
                                             where publicacion.idUsuario == idUsuario
                                             from intercambio in bd.Intercambios
                                             where (publicacion.idPublicacion == intercambio.idPublicacionUsuario1
                                             || publicacion.idPublicacion == intercambio.idPublicacionUsuario2)
                                             && intercambio.idEstado == 3
                                             select intercambio;

            return View();
        }

        [HttpGet]
        public ActionResult SolicitudRecibida(int idPublicacion1, int idPublicacion2)
        {
            var bd = new bdVentaLibrosDataContext();

            ViewBag.miLibro = from libro in bd.PublicacionIntercambio
                              where libro.idPublicacion == idPublicacion1
                              select libro;

            ViewBag.libroOfrecido = from libro in bd.PublicacionIntercambio
                                    where libro.idPublicacion == idPublicacion2
                                    select libro;

            ViewBag.idMiPublicacion = idPublicacion1;

            return View();
        }

        [HttpGet]
        public ActionResult SolicitudEnviada(int idPublicacion1, int idPublicacion2)
        {
            var bd = new bdVentaLibrosDataContext();

            ViewBag.miLibro = from libro in bd.PublicacionIntercambio
                              where libro.idPublicacion == idPublicacion2
                              select libro;

            ViewBag.libroDeseado = from libro in bd.PublicacionIntercambio
                                   where libro.idPublicacion == idPublicacion1
                                   select libro;

            ViewBag.idMiPublicacion = idPublicacion2;

            return View();
        }

        [HttpPost]
        public ActionResult CancelarIntercambio(int idPublicacion1, int idPublicacion2)
        {
            var bd = new bdVentaLibrosDataContext();

            var eliminarIntercambio = from intercambio in bd.Intercambios
                                      where intercambio.idPublicacionUsuario1 == idPublicacion1
                                      where intercambio.idPublicacionUsuario2 == idPublicacion2
                                      select intercambio;

            foreach (var intercambio in eliminarIntercambio)
            {
                bd.Intercambios.DeleteOnSubmit(intercambio);
            }

            try
            {
                bd.SubmitChanges();
                TempData["Message"] = "Intercambio cancelado";
            }
            catch (Exception e)
            {
                TempData["Message"] = "No se pudo cancelar el intercambio";
            }
            return RedirectToAction("SolicitudesIntercambio");
        }

        public ActionResult AceptarIntercambio(int idPublicacion1, int idPublicacion2)
        {
            var bd = new bdVentaLibrosDataContext();

            var actualizarIntercambio = from intercambio in bd.Intercambios
                                        where intercambio.idPublicacionUsuario1 == idPublicacion1 &&
                                        intercambio.idPublicacionUsuario2 == idPublicacion2
                                        select intercambio;

            var rechazarOtros = (from intercambio in bd.Intercambios
                                where (intercambio.idPublicacionUsuario1 == idPublicacion1 ||
                                intercambio.idPublicacionUsuario2 == idPublicacion1 ||
                                intercambio.idPublicacionUsuario1 == idPublicacion2 ||
                                intercambio.idPublicacionUsuario2 == idPublicacion2)                                
                                select intercambio).Except(actualizarIntercambio);

            var deshabilitarPublicaciones = from publicacion in bd.PublicacionIntercambio
                                            where publicacion.idPublicacion == idPublicacion1 ||
                                            publicacion.idPublicacion == idPublicacion2
                                            select publicacion;

            ViewBag.librosIntercambiados = actualizarIntercambio;

            foreach (var intercambio in actualizarIntercambio)
            {
                intercambio.idEstado = 2; // 2 => 'SOLICITUD ACEPTADA'
            }

            //Elimina los intercambios donde se ofrecian uno de los dos libros
            foreach (var intercambio in rechazarOtros)
            {
                bd.Intercambios.DeleteOnSubmit(intercambio);
            }

            //Deshabilito las publicaciones
            foreach (var publicacion in deshabilitarPublicaciones)
            {
                publicacion.idEstado = 3; //3 => 'EN INTERCAMBIO'
            }

            try
            {
                bd.SubmitChanges();
                TempData["Message"] = "Intercambio aceptado! Ingresa al detalle del intercambio para ver los datos del usuario";
            }
            catch (Exception e)
            {
                TempData["Message"] = "No se pudo aceptar el intercambio";
            }


            foreach (var intercambio in rechazarOtros)
            {
                bd.Intercambios.DeleteOnSubmit(intercambio);
            }

            return RedirectToAction("SolicitudesIntercambio");
        }

        public ActionResult IntercambioAceptado(int idPublicacion1, int idPublicacion2)
        {
            var bd = new bdVentaLibrosDataContext();

            ViewBag.libroIntercambiados = from intercambio in bd.Intercambios
                                          where intercambio.idPublicacionUsuario1 == idPublicacion1 &&
                                          intercambio.idPublicacionUsuario2 == idPublicacion2
                                          select intercambio;

            ViewBag.libro1 = from libro in bd.PublicacionIntercambio
                             where libro.idPublicacion == idPublicacion1
                             select libro;

            ViewBag.libro2 = from libro in bd.PublicacionIntercambio
                             where libro.idPublicacion == idPublicacion2
                             select libro;

            return View();
        }

        public ActionResult IntercambioRealizado(int idPublicacion1, int idPublicacion2)
        {
            var bd = new bdVentaLibrosDataContext();

            var intercambioRealizado = from intercambio in bd.Intercambios
                                       where intercambio.idPublicacionUsuario1 == idPublicacion1 &&
                                       intercambio.idPublicacionUsuario2 == idPublicacion2
                                       select intercambio;

            var publicacionUsu1 = from publicacion in bd.PublicacionIntercambio
                                  where publicacion.idPublicacion == idPublicacion1
                                  select publicacion;

            var publicacionUsu2 = from publicacion in bd.PublicacionIntercambio
                                  where publicacion.idPublicacion == idPublicacion2
                                  select publicacion;

            foreach (var intercambio in intercambioRealizado)
            {
                intercambio.idEstado = 3;
            }

            foreach (var publicacion in publicacionUsu1)
            {
                publicacion.idEstado = 2;
            }

            foreach (var publicacion in publicacionUsu2)
            {
                publicacion.idEstado = 2;
            }

            bd.SubmitChanges();

            TempData["Message"] = "Felicitaciones! El intercambio ha sido exitoso";
            return RedirectToAction("SolicitudesIntercambio");
        }

        public ActionResult IntercambioNoRealizado(int idPublicacion1, int idPublicacion2)
        {
            var bd = new bdVentaLibrosDataContext();

            var intercambioNoRealizado = from intercambio in bd.Intercambios
                                         where intercambio.idPublicacionUsuario1 == idPublicacion1 &&
                                         intercambio.idPublicacionUsuario2 == idPublicacion2
                                         select intercambio;

            var habilitarPublicaciones = from publicacion in bd.PublicacionIntercambio
                                         where publicacion.idPublicacion == idPublicacion1 ||
                                         publicacion.idPublicacion == idPublicacion2
                                         select publicacion;

            //Vuelvo a habilitar las publicaciones de los dos libros involucrados
            foreach(var publicacion in habilitarPublicaciones)
            {
                publicacion.idEstado = 1; // 1 => 'ACTIVO'
            }

            //Elimino el intercambio
            foreach (var intercambio in intercambioNoRealizado)
            {
                bd.Intercambios.DeleteOnSubmit(intercambio);
            }

            try
            {
                bd.SubmitChanges();
                TempData["Message"] = "Ups! Lamentamos el fracaso en el intercambio. Tu libro ha vuelto a publicarse";
            } 
            catch (Exception e)
            {
                TempData["Message"] = "No se pudo cancelar el intercambio. Intentelo de nuevo";
            }
            
            return RedirectToAction("SolicitudesIntercambio");
        }


    }
}

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

            var idUsuario = Convert.ToInt32(Session["idUsuario"]);

            var usuarioLogueado = (from usuario in bd.Usuarios
                                  where usuario.idUsuario == idUsuario
                                  select usuario).FirstOrDefault();

            var listadoLibrosPublicados = from publicacion in bd.PublicacionPorUsuario
                                          where publicacion.idUsuario == idUsuario
                                          select publicacion;

            return View(listadoLibrosPublicados);
        }
    }
}

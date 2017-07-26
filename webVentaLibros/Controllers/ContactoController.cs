using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class ContactoController : Controller
    {
        // GET: Contacto
        public ActionResult Contacto()
        {
            var bd = new bdVentaLibrosDataContext();

            ViewBag.tiposMensaje = from tipo in bd.TipoMensajeUsuario
                                   select tipo;

            return View();
        }

        [HttpPost]
        public ActionResult EnviarMensaje(MensajeUsuarioModel mensajeUsu)
        {
            var bd = new bdVentaLibrosDataContext();

            MensajeUsuario mensaje = new MensajeUsuario
            {
                nombre = mensajeUsu.nombre,
                email = mensajeUsu.email,
                mensaje = mensajeUsu.mensaje,
                idTipoMensaje = Convert.ToInt32(mensajeUsu.idTipoMensaje),
                fechaHora = DateTime.Now
            };

            try
            { 
            bd.MensajeUsuario.InsertOnSubmit(mensaje);
            bd.SubmitChanges();
                TempData["Message"] = "Mensaje enviado. Gracias por contactarnos.";
            }
            catch(Exception e)
            {
                TempData["Message"] = "No se puedo enviar el mensaje."+e.ToString();
            }
            return RedirectToAction("Contacto");
        }
    }
}
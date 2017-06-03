using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class GeneroController : Controller
    {
        //
        // GET: /Genero/

        [Authorize(Roles = "Administrador")]
        public ActionResult Generos()
        {
            var bd = new bdVentaLibrosDataContext();

            var listaGeneros = (from genero in bd.Generos
                                    select genero).ToList();

            return View(listaGeneros);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult AgregarGenero(GeneroModel genero)
        {
            var bd = new bdVentaLibrosDataContext();
            if (GeneroExiste(genero.nombreGenero))
            {
                TempData["Message"] = "El género literario ya existe";
            }
            else
            {
                Generos nuevoGenero = new Generos
                {
                    nombre = genero.nombreGenero
                };
                bd.Generos.InsertOnSubmit(nuevoGenero);
                bd.SubmitChanges();
                TempData["Message"] = "Género literario agregado!";
            }
            return RedirectToAction("Generos");
        }

        [Authorize(Roles = "Administrador")]
        private bool GeneroExiste(string nombre)
        {
            bool existe = false;
            using (var bd = new bdVentaLibrosDataContext())
            {
                var genero = bd.Generos.FirstOrDefault(g => g.nombre == nombre);
                if (genero != null)
                {

                    existe = true;

                }
            }
            return existe;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class EditorialController : Controller
    {
        //
        // GET: /Editorial/

        public ActionResult Editoriales()
        {
            var bd = new bdVentaLibrosDataContext();

            var listaEditoriales = (from editorial in bd.Editoriales
                                select editorial).ToList();

            return View(listaEditoriales);
        }

        [HttpPost]
        public ActionResult AgregarEditorial(EditorialModel editorial)
        {
            var bd = new bdVentaLibrosDataContext();
            if (EditorialExiste(editorial.nombre))
            {
                TempData["Message"] = "La editorial ya existe";
            }
            else
            {
                Editoriales nuevaEditorial = new Editoriales
                {
                    nombre = editorial.nombre
                };
                bd.Editoriales.InsertOnSubmit(nuevaEditorial);
                bd.SubmitChanges();
                TempData["Message"] = "Editorial agregada!";
            }
            return RedirectToAction("Editoriales");
        }

        private bool EditorialExiste(string nombre)
        {
            bool existe = false;
            using (var bd = new bdVentaLibrosDataContext())
            {
                var editorial = bd.Editoriales.FirstOrDefault(e => e.nombre == nombre);
                if (editorial != null)
                {
                    
                        existe = true;
                    
                }
            }
            return existe;
        }
    }
}

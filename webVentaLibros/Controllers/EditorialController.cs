﻿using System;
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
        [Authorize(Roles = "Administrador")]
        public ActionResult Editoriales()
        {
            var bd = new bdVentaLibrosDataContext();

            var listaEditoriales = (from editorial in bd.Editoriales
                                select editorial).ToList();

            return View(listaEditoriales);
        }

        [Authorize(Roles = "Administrador")]
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

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult ModificarEditorial(int id)
        {
            var bd = new bdVentaLibrosDataContext();

            var editorialElegida = from editorial in bd.Editoriales
                                   where editorial.idEditorial == id
                                   select editorial;

            return View(editorialElegida);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult ModificarEditorial(EditorialModel editorialElegida)
        {
            var bd = new bdVentaLibrosDataContext();

            var editorialModificada = from editorial in bd.Editoriales
                                      where editorial.idEditorial == editorialElegida.idEditorial
                                      select editorial;

            foreach (var editorial in editorialModificada)
            {
                editorial.nombre = editorialElegida.nombre;
            }

            bd.SubmitChanges();
            TempData["Message"] = "Editorial modificado!";

            return RedirectToAction("Editoriales");
        }

        [Authorize(Roles = "Administrador")]
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

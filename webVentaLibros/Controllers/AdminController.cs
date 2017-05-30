using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AgregarLibro()
        {
            var bd = new bdVentaLibrosDataContext();

            var listaGeneros = ((from genero in bd.Generos
                                select new GeneroModel
                                {
                                    idGenero = genero.idGenero,
                                    nombreGenero = genero.nombre
                                }).OrderBy(g => g.nombreGenero)).ToList();

            var listaAutores = ((from autor in bd.Autores
                                select new AutorModel
                                {
                                    idAutor = autor.idAutor,
                                    nombreAutor = autor.nombres,
                                    apellidoAutor = autor.apellidos
                                }).OrderBy(a => a.apellidoAutor)).ToList();

            var listaEditoriales = ((from editorial in bd.Editoriales
                                 select new EditorialModel
                                 {
                                     idEditorial = editorial.idEditorial,
                                     nombre = editorial.nombre
                                 }).OrderBy(e => e.nombre)).ToList();

            ViewBag.listadoAutores = listaAutores;
            ViewBag.listadoGeneros = listaGeneros;
            ViewBag.listadoEditoriales = listaEditoriales;

            return View();
        }

        [HttpPost]
        public ActionResult AgregarLibro(LibroModel libro, HttpPostedFileBase foto)
        {
            var bd = new bdVentaLibrosDataContext();

            foto.SaveAs(System.IO.Path.Combine(@"D:\webVentaLibros\webVentaLibros\img\catalogo", System.IO.Path.GetFileName(foto.FileName)));

            //Convierto el model (libro) en entidad
            Libros entidadLibro = new Libros
            {
                titulo = libro.titulo,
                codigoBarra = libro.codigoBarra,
                idEditorial = libro.idEditorial,
                paginas = libro.paginas,
                stock = libro.stock,
                edicion = libro.edicion,
                precio = Convert.ToDecimal(libro.precio),
                idGenero = libro.idGenero,
                sinopsis = libro.sinopsis,
                idAutor1 = libro.idAutor1,
                idAutor2 = libro.idAutor2,
                idAutor3 = libro.idAutor3,
                idAutor4 = libro.idAutor4,
                foto =  @"img/catalogo/"+foto.FileName,
                fechaAlta = DateTime.Now
            };


            //Agregando un nuevo registro 
            bd.Libros.InsertOnSubmit(entidadLibro);
            bd.SubmitChanges();

            return RedirectToAction("AgregarLibro");
        }

        [HttpGet]
        public ActionResult AgregarAutor()
        {
            return View();
        }
    }
}

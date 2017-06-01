﻿using System;
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

            var listaLibros = (from libro in bd.Libros
                               from editorial in bd.Editoriales
                               .Where(e => e.idEditorial == libro.idEditorial).DefaultIfEmpty()
                               from genero in bd.Generos
                               .Where(g => g.idGenero == libro.idGenero).DefaultIfEmpty()
                               from autor in bd.Autores
                               .Where(a => a.idAutor == libro.idAutor1).DefaultIfEmpty()
                               .Where(a => a.idAutor == libro.idAutor2).DefaultIfEmpty()
                               .Where(a => a.idAutor == libro.idAutor3).DefaultIfEmpty()
                               .Where(a => a.idAutor == libro.idAutor4).DefaultIfEmpty()
                               select new LibroModel
                               {
                                   codigoBarra = libro.codigoBarra,
                                   titulo = libro.titulo,
                                   genero = libro.Generos.nombre,
                                   autor1 = libro.Autores.apellidos + ", " + libro.Autores.nombres,
                                   autor2 = libro.Autores1.apellidos + ", " + libro.Autores.nombres,
                                   autor3 = libro.Autores2.apellidos + ", " + libro.Autores.nombres,
                                   autor4 = libro.Autores3.apellidos + ", " + libro.Autores.nombres,
                                   editorial = libro.Editoriales.nombre,
                                   precio = Convert.ToDouble(libro.precio)
                               }).ToList();
            /**
            var listaLibros = (from libro in bd.Libros
                               join editorial in bd.Editoriales on libro.idEditorial equals editorial.idEditorial                           
                               select libro).ToList();
            **/
            /**
            var listaLibros = (from libro in bd.Libros
                               from editorial in bd.Editoriales
                               from autor in bd.Autores
                               from genero in bd.Generos
                               where libro.idEditorial == editorial.idEditorial &&
                               libro.idGenero == genero.idGenero &&
                               libro.idAutor1 == autor.idAutor &&
                               libro.idAutor2 == autor.idAutor &&
                               libro.idAutor3 == autor.idAutor &&
                               libro.idAutor4 == autor.idAutor
                               select new LibroModel { 
                               codigoBarra = libro.codigoBarra,
                               titulo = libro.titulo,
                               genero = libro.Generos.nombre,
                               autor1 = libro.Autores.apellidos+", "+libro.Autores.nombres,
                               autor2 = libro.Autores1.apellidos + ", " + libro.Autores.nombres,
                               autor3 = libro.Autores2.apellidos + ", " + libro.Autores.nombres,
                               autor4 = libro.Autores3.apellidos + ", " + libro.Autores.nombres,
                               editorial = libro.Editoriales.nombre,
                               precio = Convert.ToDouble(libro.precio)
                               }).ToList();
            **/
            ViewBag.listadoAutores = listaAutores;
            ViewBag.listadoGeneros = listaGeneros;
            ViewBag.listadoEditoriales = listaEditoriales;
            ViewBag.listadoLibros = listaLibros;

            return View();
        }

        [HttpPost]
        public ActionResult AgregarLibro(LibroModel libro, HttpPostedFileBase foto)
        {
            var bd = new bdVentaLibrosDataContext();

            if (LibroExiste(libro.codigoBarra))
            {
                TempData["Message"] = "Ya existe un libro con el mismo título y editorial";
            }
            else
            {
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
                    foto = @"img/catalogo/" + foto.FileName,
                    fechaAlta = DateTime.Now
                };

                //Agregando un nuevo registro 
                bd.Libros.InsertOnSubmit(entidadLibro);
                bd.SubmitChanges();
                TempData["Message"] = "Libro agregado!";
            }

            return RedirectToAction("AgregarLibro");
        }

        [HttpGet]
        public ActionResult AgregarAutor()
        {
            var bd = new bdVentaLibrosDataContext();

            var listaAutores = (from autor in bd.Autores
                                select autor).ToList();

            return View(listaAutores);
        }

        [HttpPost]
        public ActionResult AgregarAutor(AutorModel autor)
        {
            var bd = new bdVentaLibrosDataContext();
            var autorIngresado = new AutorModel
            {
                apellidoAutor = autor.apellidoAutor,
                nombreAutor = autor.nombreAutor
            };

            if (AutorExiste(autorIngresado.apellidoAutor, autorIngresado.nombreAutor))
            {
                TempData["Message"] = "Ya existe un autor con esos datos";
            }
            else
            {
                Autores nuevoAutor = new Autores
                {
                    apellidos = autor.apellidoAutor,
                    nombres = autor.nombreAutor
                };
                bd.Autores.InsertOnSubmit(nuevoAutor);
                bd.SubmitChanges();
                TempData["Message"] = "Autor agregado!";
            }

            return RedirectToAction("AgregarAutor");
        }

        //Verifiar si el autor ya existe
        private bool AutorExiste(string apellidos, string nombres)
        {
            bool existe = false;
            using (var bd = new bdVentaLibrosDataContext())
            {
                var autor = bd.Autores.FirstOrDefault(a => a.apellidos == apellidos);
                if (autor != null)
                {
                    if (autor.nombres == nombres) //Verificar nombres del autor
                    {
                        existe = true;
                    }
                }
            }
            return existe;
        }

        private bool LibroExiste(string codigoBarra)
        {
            bool existe = false;
            using (var bd = new bdVentaLibrosDataContext())
            {
                var libro = bd.Libros.FirstOrDefault(l => l.codigoBarra == codigoBarra);
                if (libro != null)
                {

                    existe = true;
                }
            }
            return existe;
        }
    }
}

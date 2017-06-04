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
        [Authorize(Roles="Administrador")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
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
            
            ViewBag.listadoAutores = listaAutores;
            ViewBag.listadoGeneros = listaGeneros;
            ViewBag.listadoEditoriales = listaEditoriales;
            ViewBag.listadoLibros = listaLibros;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public ActionResult ModificarLibro(string codBarra)
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

            var libroElegido = from libro in bd.Libros
                               where codBarra == libro.codigoBarra
                               from editorial in bd.Editoriales
                               .Where(e => e.idEditorial == libro.idEditorial).DefaultIfEmpty()
                               from genero in bd.Generos
                               .Where(g => g.idGenero == libro.idGenero).DefaultIfEmpty()
                               from autor in bd.Autores
                               .Where(a => a.idAutor == libro.idAutor1).DefaultIfEmpty()
                               .Where(a => a.idAutor == libro.idAutor2).DefaultIfEmpty()
                               .Where(a => a.idAutor == libro.idAutor3).DefaultIfEmpty()
                               .Where(a => a.idAutor == libro.idAutor4).DefaultIfEmpty()
                               select new LibroModel{
                                   codigoBarra = libro.codigoBarra,
                                   titulo = libro.titulo,
                                   idAutor1 = libro.Autores.idAutor,
                                   idAutor2 = libro.Autores1.idAutor,
                                   idAutor3 = libro.Autores2.idAutor,
                                   idAutor4 = libro.Autores3.idAutor,
                                   autor1 = libro.Autores.apellidos + ", " + libro.Autores.nombres,
                                   autor2 = libro.Autores1.apellidos + ", " + libro.Autores.nombres,
                                   autor3 = libro.Autores2.apellidos + ", " + libro.Autores.nombres,
                                   autor4 = libro.Autores3.apellidos + ", " + libro.Autores.nombres,
                                   idGenero = libro.Generos.idGenero,
                                   genero = libro.Generos.nombre,
                                   idEditorial = libro.Editoriales.idEditorial,
                                   editorial = libro.Editoriales.nombre,
                                   sinopsis = libro.sinopsis,
                                   precio = Convert.ToDouble(libro.precio),
                                   paginas = libro.paginas,
                                   foto = libro.foto,
                                   edicion = libro.edicion
                               };

            return View(libroElegido);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public ActionResult ModificarLibro(LibroModel libroElegido, HttpPostedFileBase foto)
        {
            var bd = new bdVentaLibrosDataContext();

            if (foto != null) { 
                foto.SaveAs(System.IO.Path.Combine(@"D:\webVentaLibros\webVentaLibros\img\catalogo", System.IO.Path.GetFileName(foto.FileName)));
            }

            var libroModificado = from libro in bd.Libros
                                  where libroElegido.codigoBarra == libro.codigoBarra
                                  select libro;                     

            //Cambio los datos del inmueble
            if (foto != null)
            {
                foreach (var libro in libroModificado)
                {
                    libro.titulo = libroElegido.titulo;
                    libro.idGenero = libroElegido.idGenero;
                    libro.idEditorial = libroElegido.idEditorial;
                    libro.idAutor1 = libroElegido.idAutor1;
                    libro.idAutor2 = libroElegido.idAutor2;
                    libro.idAutor3 = libroElegido.idAutor3;
                    libro.idAutor4 = libroElegido.idAutor4;
                    libro.paginas = libroElegido.paginas;
                    libro.precio = Convert.ToDecimal(libroElegido.precio);
                    libro.sinopsis = libroElegido.sinopsis;
                    libro.edicion = libroElegido.edicion;
                    libro.foto = @"img/catalogo/" + foto.FileName;
                }
            }
            else
            {
                foreach (var libro in libroModificado)
                {
                    libro.titulo = libroElegido.titulo;
                    libro.idGenero = libroElegido.idGenero;
                    libro.idEditorial = libroElegido.idEditorial;
                    libro.idAutor1 = libroElegido.idAutor1;
                    libro.idAutor2 = libroElegido.idAutor2;
                    libro.idAutor3 = libroElegido.idAutor3;
                    libro.idAutor4 = libroElegido.idAutor4;
                    libro.paginas = libroElegido.paginas;
                    libro.precio = Convert.ToDecimal(libroElegido.precio);
                    libro.sinopsis = libroElegido.sinopsis;
                    libro.edicion = libroElegido.edicion;
                }
            }
            //Hago el submit
            bd.SubmitChanges();
            TempData["Message"] = "Libro modificado!";
            return RedirectToAction("AgregarLibro");
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult AgregarAutor()
        {
            var bd = new bdVentaLibrosDataContext();

            var listaAutores = (from autor in bd.Autores
                                select autor).ToList();

            return View(listaAutores);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
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

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult ModificarAutor(int id)
        {
            var bd = new bdVentaLibrosDataContext();

            var autorElegido = from autor in bd.Autores
                               where autor.idAutor == id
                               select autor;

            return View(autorElegido);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult ModificarAutor(AutorModel autorElegido)
        {
            var bd = new bdVentaLibrosDataContext();

            var autorModificado = from autor in bd.Autores
                                  where autor.idAutor == autorElegido.idAutor
                                  select autor;

            foreach (var autor in autorModificado)
            {
                autor.apellidos = autorElegido.apellidoAutor;
                autor.nombres = autorElegido.nombreAutor;
            }

            bd.SubmitChanges();
            TempData["Message"] = "Autor modificado!";

            return RedirectToAction("AgregarAutor");
        }

        [Authorize(Roles = "Administrador")]
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

        [Authorize(Roles = "Administrador")]
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

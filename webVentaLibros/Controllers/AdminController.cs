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
        [Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            var bd = new bdVentaLibrosDataContext();

            ViewBag.totalLibros = (from libro in bd.Libros
                                   where libro.stock > 0
                                   select libro).Count();

            //ventas concretadas en el mes actual
            ViewBag.totalVentasConcretadas = (from pedido in bd.Pedidos
                                              where pedido.idEstadoPedido == 5
                                              && pedido.fechaHora.Value.Year == DateTime.Now.Year
                                              && pedido.fechaHora.Value.Month == DateTime.Now.Month
                                              select pedido).Count();

            ViewBag.totalPublicacionesIntercambio = (from publicacion in bd.PublicacionIntercambio
                                                     where publicacion.idEstado == 1
                                                     select publicacion).Count();

            //intercambios contreados en el mes actual
            ViewBag.totalIntercambiosConcretados = (from intercambio in bd.Intercambios
                                                    where intercambio.idEstado == 3
                                                    && intercambio.fechaHora.Year == DateTime.Now.Year
                                                    && intercambio.fechaHora.Month == DateTime.Now.Month
                                                    select intercambio).Count();

            //pedidos pagados por el cliente pendientes para envio (los 5 mas antiguos en fecha de pedido)
            ViewBag.pedidosPagados = (from pedido in bd.Pedidos
                                      where pedido.idEstadoPedido == 2
                                      select pedido).OrderBy(x => x.fechaHora).Take(5).ToList();

            //libros con stock minimo o menos (los 5 con menos stock)
            ViewBag.librosStockMinimo = (from libro in bd.Libros
                                         where libro.stock < 30
                                         select libro).OrderBy(x => x.stock).Take(5).ToList();

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
                foto.SaveAs(System.IO.Path.Combine(@"E:\webVentaLibros\webVentaLibros\img\catalogo", System.IO.Path.GetFileName(foto.FileName)));

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
                               select new LibroModel
                               {
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

            if (foto != null)
            {
                foto.SaveAs(System.IO.Path.Combine(@"E:\webVentaLibros\webVentaLibros\img\catalogo", System.IO.Path.GetFileName(foto.FileName)));
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

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult ActualizarStock()
        {
            var bd = new bdVentaLibrosDataContext();

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
                                   foto = libro.foto,
                                   titulo = libro.titulo,
                                   genero = libro.Generos.nombre,
                                   autor1 = libro.Autores.apellidos + ", " + libro.Autores.nombres,
                                   autor2 = libro.Autores1.apellidos + ", " + libro.Autores.nombres,
                                   autor3 = libro.Autores2.apellidos + ", " + libro.Autores.nombres,
                                   autor4 = libro.Autores3.apellidos + ", " + libro.Autores.nombres,
                                   editorial = libro.Editoriales.nombre,
                                   precio = Convert.ToDouble(libro.precio),
                                   stock = libro.stock
                               }).ToList();

            ViewBag.listadoLibros = listaLibros;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult ActualizarStock(string codBarra, int stockIngreso)
        {
            var bd = new bdVentaLibrosDataContext();

            var libroActualizarStock = from libro in bd.Libros
                                       where libro.codigoBarra == codBarra
                                       select libro;

            foreach (var libro in libroActualizarStock)
            {
                libro.stock = libro.stock + stockIngreso;
            }

            try
            {
                bd.SubmitChanges();
                TempData["Message"] = "Stock de ISBN " + codBarra + " fue actualizado";

                ViewBag.listadoLibros = (from libro in bd.Libros
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
                                             foto = libro.foto,
                                             titulo = libro.titulo,
                                             genero = libro.Generos.nombre,
                                             autor1 = libro.Autores.apellidos + ", " + libro.Autores.nombres,
                                             autor2 = libro.Autores1.apellidos + ", " + libro.Autores.nombres,
                                             autor3 = libro.Autores2.apellidos + ", " + libro.Autores.nombres,
                                             autor4 = libro.Autores3.apellidos + ", " + libro.Autores.nombres,
                                             editorial = libro.Editoriales.nombre,
                                             precio = Convert.ToDouble(libro.precio),
                                             stock = libro.stock
                                         }).ToList();
            }
            catch (Exception ex)
            {
                ViewBag.listadoLibros = (from libro in bd.Libros
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
                                             foto = libro.foto,
                                             titulo = libro.titulo,
                                             genero = libro.Generos.nombre,
                                             autor1 = libro.Autores.apellidos + ", " + libro.Autores.nombres,
                                             autor2 = libro.Autores1.apellidos + ", " + libro.Autores.nombres,
                                             autor3 = libro.Autores2.apellidos + ", " + libro.Autores.nombres,
                                             autor4 = libro.Autores3.apellidos + ", " + libro.Autores.nombres,
                                             editorial = libro.Editoriales.nombre,
                                             precio = Convert.ToDouble(libro.precio),
                                             stock = libro.stock
                                         }).ToList();
                TempData["Message"] = "No se pudo actualizar el stock. Intente nuevamente";
            }

            return View();
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

        [Authorize(Roles = "Administrador")]
        public ActionResult Mensajes()
        {
            var bd = new bdVentaLibrosDataContext();

            ViewBag.mensajes = (from mensaje in bd.MensajeUsuario
                                select mensaje).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult EliminarMensaje(int id)
        {
            var bd = new bdVentaLibrosDataContext();

            var mensaje = (from m in bd.MensajeUsuario
                           where m.idMensaje == id
                           select m).ToList();

            foreach (var m in mensaje)
            {
                bd.MensajeUsuario.DeleteOnSubmit(m);
            }

            try
            {
                bd.SubmitChanges();
                TempData["Message"] = "Mensaje eliminado";
            }
            catch (Exception e)
            {
                TempData["Message"] = "No se pudo eliminar el mensaje";
            }

            return RedirectToAction("Mensajes");
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult ReporteVentas(DateTime inicio, DateTime fin)
        {
            var bd = new bdVentaLibrosDataContext();
            ViewBag.fechaDesde = inicio.ToShortDateString();
            ViewBag.fechaHasta = fin.ToShortDateString();
            double totalIngresos = 0;
            int totalLibrosVendidos = 0;

            ViewBag.ventasPeriodo = (from pedido in bd.Pedidos
                                     where pedido.idEstadoPedido == 5
                                     && (pedido.fechaHora >= inicio && pedido.fechaHora <= fin)
                                     select pedido
                                     ).ToList();

            if (ViewBag.ventasPeriodo.Count > 0)
            {
                foreach (var venta in ViewBag.ventasPeriodo)
                {
                    foreach(var detalle in venta.DetallePorPedido)
                    {
                        totalIngresos = totalIngresos + (Convert.ToDouble(detalle.precioUnitario) * detalle.cantidad);
                        totalLibrosVendidos = totalLibrosVendidos + detalle.cantidad;
                    }
                }
            }
            ViewBag.totalIngresos = totalIngresos;
            ViewBag.totalLibrosVendidos = totalLibrosVendidos;

            ViewBag.masVendidoPeriodo = (from libroVendido in bd.DetallePorPedido
                                         from pedido in bd.Pedidos
                                         where libroVendido.idPedido == pedido.idPedido
                                         && (pedido.fechaHora >= inicio && pedido.fechaHora <= fin)
                                         from libro in bd.Libros
                                         where libro.codigoBarra == libroVendido.codigoLibro
                                         group libroVendido by libroVendido.codigoLibro into g
                                         orderby g.Sum(x => x.cantidad) descending
                                         select g.First()).Take(5);

            ViewBag.generosMasVendidos = (from pedido in bd.Pedidos
                                          where pedido.fechaHora >= inicio && pedido.fechaHora <= fin
                                          from detalle in bd.DetallePorPedido
                                          where detalle.idPedido == pedido.idPedido
                                          from libro in bd.Libros
                                          where detalle.codigoLibro == libro.codigoBarra
                                          from genero in bd.Generos
                                          where detalle.Libros.idGenero == genero.idGenero
                                          group genero by genero.nombre into g
                                          orderby g.Count() descending
                                          select g.First()).Take(3).ToList();
            return View();
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult ReportesVarios(DateTime inicio, DateTime fin)
        {
            var bd = new bdVentaLibrosDataContext();
            ViewBag.fechaDesde = inicio.ToShortDateString();
            ViewBag.fechaHasta = fin.ToShortDateString();

            ViewBag.cantidadIntercambios = (from intercambio in bd.Intercambios
                                            where intercambio.fechaHora >= inicio && intercambio.fechaHora <= fin
                                            select intercambio).Count();

            ViewBag.cantidadIntercambiosPublicados = (from publicacion in bd.PublicacionIntercambio
                                                      where publicacion.fechaHoraAlta >= inicio && publicacion.fechaHoraAlta <= fin
                                                      select publicacion).Count();

            ViewBag.usuariosRegistrados = (from usuario in bd.Usuarios
                                           where usuario.fechaHoraAlta >= inicio && usuario.fechaHoraAlta <= fin
                                           select usuario).Count();

            ViewBag.masDeseados = (from deseado in bd.ListaDeseados
                                   where deseado.fechaHora >= inicio && deseado.fechaHora <= fin
                                   group deseado by deseado.Libros.codigoBarra into g
                                   orderby g.Count() descending
                                   select g.First()).Take(5).ToList();

            ViewBag.cantidadLibrosNuevos = (from libro in bd.Libros
                                            where libro.fechaAlta >= inicio && libro.fechaAlta <= fin
                                            select libro).Count();

            ViewBag.cantidadLibrosSinStock = (from libro in bd.Libros
                                              where libro.stock == 0
                                              select libro).Count();

            return View();
        }
    }
}

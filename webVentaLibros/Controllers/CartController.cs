using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class CartController : Controller
    {
        //
        // GET: /Cart/
        [HttpGet]
        public ActionResult Carrito()
        {
            double totalCompra = 0;
            if (Session["carrito"] != null)
            {
                foreach (var item in Session["carrito"] as List<CarritoItem>)
                {
                    totalCompra = totalCompra + (item.Cantidad * item.Libro.precio);
                }
                ViewBag.total = totalCompra;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Carrito(string codLibro, int quantity)
        {
            double totalCompra = 0;
            int cantidadLibros = 0;

            var bd = new bdVentaLibrosDataContext();

            //sugerencias
            var libroElegido = (from libro in bd.Libros
                                where libro.codigoBarra == codLibro
                                select libro).FirstOrDefault();
            var librosRelacionados = (from libro in bd.Libros
                                      where (libro.idGenero == libroElegido.idGenero
                                      || libro.idAutor1 == libroElegido.idAutor1)
                                      && libro.codigoBarra != libroElegido.codigoBarra
                                      orderby libro.codigoBarra
                                      select libro).Take(4);

            if (Session["sugerencias"] == null)
            {
                List<Libros> sugerencias = new List<Libros>();
                foreach (var libro in librosRelacionados)
                {
                    sugerencias.Add(libro);
                }
                Session["sugerencias"] = sugerencias;
            }
            else
            {
                List<Libros> sugerencias = (List<Libros>)Session["sugerencias"];
                int indexExistente = getIndex(codLibro);
                if (indexExistente == -1)
                    foreach (var libro in librosRelacionados)
                    {
                        sugerencias.Add(libro);
                    }

                Session["sugerencias"] = sugerencias;
            }
            //fin sugerencias

            var libroAgregar = from libro in bd.Libros
                               where libro.codigoBarra == codLibro
                               select new LibroModel
                               {
                                   codigoBarra = libro.codigoBarra,
                                   foto = libro.foto,
                                   titulo = libro.titulo,
                                   precio = Convert.ToDouble(libro.precio)
                               };
            var lista = libroAgregar.ToList();

            if (Session["carrito"] == null)
            {
                List<CarritoItem> compra = new List<CarritoItem>();
                compra.Add(new CarritoItem(lista.First(), quantity));
                Session["carrito"] = compra;

                //cantidad en carrito
                Session["cantidadlibros"] = quantity;
            }
            else
            {
                List<CarritoItem> compra = (List<CarritoItem>)Session["carrito"];
                int indexExistente = getIndex(codLibro);
                if (indexExistente == -1)
                    compra.Add(new CarritoItem(lista.First(), quantity));
                else
                    compra[indexExistente].Cantidad++;
                Session["carrito"] = compra;
            }

            foreach (var item in Session["carrito"] as List<CarritoItem>)
            {
                totalCompra = totalCompra + (item.Cantidad * item.Libro.precio);
                cantidadLibros = cantidadLibros + item.Cantidad;
            }
            ViewBag.total = totalCompra;

            Session["totalCompra"] = totalCompra;
            Session["cantidadLibros"] = cantidadLibros;

            return View();
        }

        public ActionResult Eliminar(string codLibro)
        {
            double totalCompra = 0;
            int cantidadLibros = 0;
            List<CarritoItem> compra = (List<CarritoItem>)Session["carrito"];
            compra.RemoveAt(getIndex(codLibro));
            foreach (var item in Session["carrito"] as List<CarritoItem>)
            {
                totalCompra = totalCompra + (item.Cantidad * item.Libro.precio);
                cantidadLibros = cantidadLibros + item.Cantidad;
            }
            ViewBag.total = totalCompra;

            Session["totalCompra"] = totalCompra;
            Session["cantidadLibros"] = cantidadLibros;

            return View("Carrito");

        }

        public ActionResult DatosEnvio()
        {
            //para datos del usuario
            var bd = new bdVentaLibrosDataContext();
            int idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);
            var usuarioLogueado = from usuario in bd.Usuarios
                                  where usuario.idUsuario == idUsuario
                                  select usuario;

            Usuarios us = usuarioLogueado.FirstOrDefault();
            ViewBag.usuarioLogueado = usuarioLogueado;            

            var listadoProvincias = (from p in bd.Provincias
                                     select new ProvinciaModel
                                     {
                                         idProvincia = p.idProvincia,
                                         nombreProvincia = p.nombre

                                     }).ToList();
            List<SelectListItem> li = new List<SelectListItem>();

            li.Add(new SelectListItem { Text = "Seleccione..", Value = "0", Disabled = true }); //Valor 0 por defecto..

            foreach (var p in listadoProvincias)
            {
                if (p.idProvincia == us.idProvincia)
                {
                    li.Add(new SelectListItem { Text = p.nombreProvincia, Value = p.idProvincia.ToString(), Selected = true });
                }
                else
                {
                    li.Add(new SelectListItem { Text = p.nombreProvincia, Value = p.idProvincia.ToString() });
                }
            }

            ViewData["provincias"] = li;

            //LOCALIDADES DE LA PROVINCIA (PROVINCIA INGRESADA ANTERIORMENTE)

            var prov = us.idProvincia;

            List<SelectListItem> localidades = new List<SelectListItem>();

            if (prov > 0)
            {


                var listaLocalidades = (from l in bd.Localidades
                                        where l.idProvincia == prov
                                        select new LocalidadModel
                                        {
                                            idLocalidad = l.idLocalidad,
                                            nombreLocalidad = l.nombre
                                        }).ToList();

                foreach (var loc in listaLocalidades)
                {
                    if (loc.idLocalidad == us.idLocalidad)
                    {
                        localidades.Add(new SelectListItem { Text = loc.nombreLocalidad, Value = loc.idLocalidad.ToString(), Selected = true });
                    }
                    else
                    {
                        localidades.Add(new SelectListItem { Text = loc.nombreLocalidad, Value = loc.idLocalidad.ToString() });
                    }
                }
                ViewData["localidades"] = localidades;
            }
            else
            {
                localidades.Clear();
            }

            //fin datos del usuario
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(string nombreUsuario, string apellidoUsuario, int idProvincia, int idLocalidad, string codigoPostal, string mail, string direccion)
        {

            var bd = new bdVentaLibrosDataContext();
            int idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);

            string nombreProvincia = (from provincia in bd.Provincias
                                      where provincia.idProvincia == idProvincia
                                      select provincia.nombre).FirstOrDefault();

            string nombreLocalidad = (from localidad in bd.Localidades
                                      where localidad.idLocalidad == idLocalidad
                                      select localidad.nombre).FirstOrDefault();

            DatosEnvioModel envio = new DatosEnvioModel
            {
                nombreUsuario = nombreUsuario,
                apellidoUsuario = apellidoUsuario,
                provincia = nombreProvincia,
                localidad = nombreLocalidad,
                codigoPostal = codigoPostal,
                mail = mail,
                direccion = direccion
            };

            Session["DatosEnvio"] = envio;
            ViewBag.datosEnvio = envio;

            double totalCompra = 0;
            List<CarritoItem> compra = (List<CarritoItem>)Session["carrito"];
            int cantidad = compra.Count;
            if (Session["carrito"] != null)
            {
                foreach (var item in Session["carrito"] as List<CarritoItem>)
                {
                    totalCompra = totalCompra + (item.Cantidad * item.Libro.precio);
                }
                ViewBag.total = totalCompra;
                ViewBag.cantidad = cantidad;
            }

            Pedidos nuevoPedido = new Pedidos
            {
                idUsuario = idUsuario,
                fechaHora = DateTime.Now,
                fechaEntrega = DateTime.Now.AddDays(10),
                direccionEntrega = direccion,
                idLocalidadEntrega = idLocalidad,
                idProvinciaEntrega = idProvincia,
                idEstadoPedido = 1, //en espera de pago
                codigoPostalEntrega = codigoPostal,
                total = Convert.ToDecimal(totalCompra),
                nombre = nombreUsuario,
                apellido = apellidoUsuario,
                email = mail
            };

            try
            {
                bd.Pedidos.InsertOnSubmit(nuevoPedido);
                bd.SubmitChanges();
            }
            catch (Exception e)
            {
                TempData["Message"] = "No se pudo ingresar el pedido";
            }

            var ultimoPedido = (from pedido in bd.Pedidos
                                where pedido.idUsuario == idUsuario
                                orderby pedido.fechaHora descending
                                select pedido).FirstOrDefault();
            ViewBag.pedido = ultimoPedido;

            if (Session["carrito"] != null)
            {
                foreach (var item in Session["carrito"] as List<CarritoItem>)
                {
                    DetallePorPedido detalle = new DetallePorPedido
                    {
                        idPedido = ultimoPedido.idPedido,
                        codigoLibro = item.Libro.codigoBarra,
                        cantidad = item.Cantidad,
                        precioUnitario = Convert.ToDecimal(item.Libro.precio)
                    };

                    bd.DetallePorPedido.InsertOnSubmit(detalle);
                }
                bd.SubmitChanges();
            }

            return View();
        }

        [HttpGet]
        public ActionResult Checkout(int idDescuento)
        {
            var bd = new bdVentaLibrosDataContext();
            int idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);
            ViewBag.datosEnvio = (DatosEnvioModel)Session["DatosEnvio"];

            double totalCompra = 0;
            List<CarritoItem> compra = (List<CarritoItem>)Session["carrito"];
            int cantidad = compra.Count;
            if (Session["carrito"] != null)
            {
                foreach (var item in Session["carrito"] as List<CarritoItem>)
                {
                    totalCompra = totalCompra + (item.Cantidad * item.Libro.precio);
                }
                ViewBag.total = totalCompra;
                ViewBag.cantidad = cantidad;
            }

            var ultimoPedido = (from pedido in bd.Pedidos
                                where pedido.idUsuario == idUsuario
                                orderby pedido.fechaHora descending
                                select pedido).FirstOrDefault();
            ViewBag.pedido = ultimoPedido;

            var descuentoAplicable = (from descuento in bd.Descuentos
                                      where descuento.idDescuento == idDescuento
                                      select descuento).FirstOrDefault();

            ViewBag.descuento = descuentoAplicable;

            if (idDescuento > 0)
            {
                ultimoPedido.idDescuento = descuentoAplicable.idDescuento;
                if (descuentoAplicable.idTipo == 1)
                {
                    ultimoPedido.total = ultimoPedido.total - (ultimoPedido.total * descuentoAplicable.descuento / 100);
                }
                else
                {
                    ultimoPedido.total = ultimoPedido.total - descuentoAplicable.descuento;
                }

                try
                {
                    bd.SubmitChanges();
                    TempData["Message"] = "Se ha aplicado el descuento";
                }
                catch (Exception e)
                {
                    TempData["Message"] = "El código ingresado no es correcto o ha caducado";
                }
            }

            return View();
        }

        public ActionResult AplicarDescuento(string cupon)
        {
            var bd = new bdVentaLibrosDataContext();
            var id = -1;            

            var descuentoElegido = (from descuento in bd.Descuentos
                              where descuento.validez == 1 &&
                              descuento.codigo == cupon
                              select descuento).FirstOrDefault();

            if(descuentoElegido != null)
            { 
                id = descuentoElegido.idDescuento;
            }

            return RedirectToAction("Checkout", new { idDescuento = id});
        }

        public ActionResult CompraFinalizada(int? collection_id, string collection_status, string preference_id, string external_reference, string payment_type, int? merchant_order_id)
        {
            var bd = new bdVentaLibrosDataContext();
            int idUsuario = Convert.ToInt32(System.Web.HttpContext.Current.Session["IDUSUARIO"]);

            var ultimoPedido = (from pedido in bd.Pedidos
                                where pedido.idUsuario == idUsuario
                                orderby pedido.fechaEntrega descending
                                select pedido).FirstOrDefault();

            //if (collection_status == "success")
            //{
                ultimoPedido.idEstadoPedido = 2;
            Session["carrito"] = null;

                try
                {
                    bd.SubmitChanges();
                    TempData["Message"] = "Gracias por su compra";
                }
                catch (Exception e)
                {
                    TempData["Message"] = "No se pudo registrar el pago";
                }
            //}
            return View();

        }

        private int getIndex(string codLibro)
        {
            List<CarritoItem> compra = (List<CarritoItem>)Session["carrito"];
            for (int i = 0; i < compra.Count; i++)
            {
                if (compra[i].Libro.codigoBarra == codLibro)
                    return i;
            }
            return -1;
        }

        public JsonResult GetLocalidades(string idProvincia)
        {
            var bd = new bdVentaLibrosDataContext();
            var prov = Convert.ToInt32(idProvincia);

            List<SelectListItem> localidades = new List<SelectListItem>();

            if (prov > 0)
            {


                var listaLocalidades = (from l in bd.Localidades
                                        where l.idProvincia == prov
                                        select new LocalidadModel
                                        {
                                            idLocalidad = l.idLocalidad,
                                            nombreLocalidad = l.nombre
                                        }).ToList();

                foreach (var loc in listaLocalidades)
                {
                    localidades.Add(new SelectListItem { Text = loc.nombreLocalidad, Value = loc.idLocalidad.ToString() });
                }
            }
            else
            {
                localidades.Clear();
            }
            return Json(new SelectList(localidades, "Value", "Text"));
        }
    }
}

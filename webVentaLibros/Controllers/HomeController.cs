using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            //Creo el contexto de datos 
            var bd = new bdVentaLibrosDataContext();
            //Recupero la lista de libros -> son de tipo Libro: entidad de dominio

            var lista = (from libro in bd.Libros
                         select new LibroModel
                         {
                             codigoBarra = libro.codigoBarra,
                             foto = libro.foto,
                             titulo = libro.titulo, 
                             sinopsis = libro.sinopsis,
                             precio = Convert.ToDouble(libro.precio),
                             fechaAlta = Convert.ToDateTime(libro.fechaAlta)
                         }).OrderByDescending(l => l.fechaAlta).Take(10).ToList();

            var listaCompleta = new List<LibroModel>();

            foreach (var libro in lista)
            {
                var entidadLibro = new LibroModel
                {
                    codigoBarra = libro.codigoBarra,
                    foto = libro.foto,
                    titulo = libro.titulo,
                    sinopsis = libro.sinopsis,
                    precio = libro.precio
                };
                listaCompleta.Add(entidadLibro);
            }

            ViewBag.masVendidos = (from libro in bd.DetallePorPedido
                                   from libroExistencia in bd.Libros
                                   where libro.codigoLibro == libroExistencia.codigoBarra
                                   && libroExistencia.stock > 0
                                  group libro by libro.codigoLibro into g
                                  orderby g.Sum(x => x.cantidad) descending
                                  select g.First()).Take(3);

            ViewBag.mejorCalificacion = (from libro in bd.CalificacionPorLibro
                                         from libroExistencia in bd.Libros
                                         where libro.codigoLibro == libroExistencia.codigoBarra
                                         && libroExistencia.stock > 0
                                         group libro by libro.codigoLibro into g
                                         orderby g.Average(x => x.calificacion) descending
                                         select g.First()).Take(3);

            ViewBag.masDeseados = (from libro in bd.ListaDeseados
                                   from libroExistencia in bd.Libros
                                   where libro.codigoLibro == libroExistencia.codigoBarra
                                   && libroExistencia.stock > 0
                                   group libro by libro.codigoLibro into g
                                   orderby g.Count() descending
                                   select g.First()).Take(3);
           
            //Paso como parámetro a la vista la lista de inmuebles
            return View(listaCompleta);

        }

    }
}

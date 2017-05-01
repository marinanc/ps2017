﻿using System;
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
            var dbLibros = new bdVentaLibrosDataContext();
            //Recupero la lista de inmuebles -> son de tipo Inmueble: entidad de dominio
            //var listaInmuebles = dbInmuebles.Inmueble.ToList();

            var lista = (from libro in dbLibros.Libros
                         select new LibroModel
                         {
                             foto = libro.foto,
                             titulo = libro.titulo, 
                             sinopsis = libro.sinopsis,
                             precio = Convert.ToDouble(libro.precio)
                         }).ToList();

            var listaCompleta = new List<LibroModel>();

            foreach (var libro in lista)
            {
                var entidadLibro = new LibroModel
                {
                    foto = libro.foto,
                    titulo = libro.titulo,
                    sinopsis = libro.sinopsis,
                    precio = libro.precio
                };
                listaCompleta.Add(entidadLibro);
            }
           
            //Paso como parámetro a la vista la lista de inmuebles
            return View(listaCompleta);

        }

    }
}

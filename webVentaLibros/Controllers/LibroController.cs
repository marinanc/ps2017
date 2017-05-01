using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace webVentaLibros.Controllers
{
    public class LibroController : Controller
    {
        //
        // GET: /Libro/

        public ActionResult Index()
        {
            return View();
        }

    }
}

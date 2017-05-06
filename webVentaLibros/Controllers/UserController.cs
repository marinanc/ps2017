using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using webVentaLibros.Models;

namespace webVentaLibros.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserModel user)
        {
            if (ModelState.IsValid) //Verificar que el modelo de datos sea válido en cuanto a la definición de las propiedades
            {
                if (Isvalid(user.usuario,user.contraseña))//Verificar que el usuario y clave exista utilizando el método privado
                {
                    FormsAuthentication.SetAuthCookie(user.usuario, false); //crea variable de user con el usuario
                    return RedirectToAction("Index", "Home"); //dirigir al controlador home vista Index una vez se a autenticado en el sistema
                }
                else
                {
                    ModelState.AddModelError("", "Datos incorrectos"); //Mensaje de error si no usuario y contraseña estan mal
                }
            }
            return View(user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut(); //cierrar sesion
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public ActionResult CambiarContraseña()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CambiarContraseña(UserModel userModel)
        {
            var bdVentaLibros = new bdVentaLibrosDataContext();

            var updateContraseña =
                        from user in bdVentaLibros.Usuarios
                        where user.mail == userModel.usuario
                        select user;

            
            if(updateContraseña != null)
            {
                foreach(var user in updateContraseña)
                {
                    user.contraseña = userModel.contraseña;
                }

                bdVentaLibros.SubmitChanges();
            }
            else
            {
                 ModelState.AddModelError("", "No se realizo cambio de contraseña"); //Mensaje de error si no realizo el cambio de contraseña
            }

            //return RedirectToAction("Index","Home");
            return View();
        }

        // Metodo para validar el usuario y password del usuario, realiza la consulta a la base de datos
        private bool Isvalid(string usuario, string contraseña)
        {
            bool Isvalid = false;
            using (var bd = new bdVentaLibrosDataContext())
            {
                var user = bd.Usuarios.FirstOrDefault(u => u.mail == usuario); 
                if (user !=null)
                {
                    if (user.contraseña == contraseña) //Verificar contraseña del usuario
                    {
                        Isvalid = true;
                    }
                }
            }
            return Isvalid;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using webVentaLibros.Models;
using WebMatrix.WebData;

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
            var bd = new bdVentaLibrosDataContext();

            if (ModelState.IsValid) //Verificar que el modelo de datos sea válido en cuanto a la definición de las propiedades
            {
                if (Isvalid(user.mail, user.contraseña))//Verificar que el usuario y clave exista utilizando el método privado IsValid()
                {
                    var usuarioLogueado = from us in bd.Usuarios
                                          where us.mail == user.mail
                                          select new UserModel
                                          {
                                              idUsuario = us.idUsuario,
                                              mail = us.mail,
                                              nombreUsuario = us.nombreUsuario,
                                              idPerfil = us.idPerfil
                                          };
                    if (usuarioLogueado.FirstOrDefault().idPerfil == 1)
                    {
                        WebMatrix.WebData.WebSecurity.Login("admin@admin.com", user.contraseña);
                    }
                    else
                    {
                        WebMatrix.WebData.WebSecurity.Login(usuarioLogueado.FirstOrDefault().mail, user.contraseña);
                    }
                    System.Web.HttpContext.Current.Session["IDUSUARIO"] = usuarioLogueado.FirstOrDefault().idUsuario;
                    System.Web.HttpContext.Current.Session["nombreUsuario"] = usuarioLogueado.FirstOrDefault().nombreUsuario;
                    Session["idUsuario"] = usuarioLogueado.FirstOrDefault().idUsuario;
                    FormsAuthentication.SetAuthCookie(usuarioLogueado.FirstOrDefault().mail, false); //crea variable de user con el usuario



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
            //if (WebSecurity.CurrentUserName == "Admin")
            //{
            WebSecurity.Logout();
            //}
            FormsAuthentication.SignOut(); //cerrar sesion
            System.Web.HttpContext.Current.Session["IDUSUARIO"] = null;
            return RedirectToAction("Index", "Home");
        }        

        public ActionResult RegistrarUsuario()
        {
            var bd = new bdVentaLibrosDataContext();

            var listadoProvincias = (from p in bd.Provincias
                                     select new ProvinciaModel
                                     {
                                         idProvincia = p.idProvincia,
                                         nombreProvincia = p.nombre

                                     }).ToList();
            List<SelectListItem> li = new List<SelectListItem>();

            li.Add(new SelectListItem { Text = "Seleccione..", Value = "0" }); //Valor 0 por defecto..

            foreach (var p in listadoProvincias)
            {
                li.Add(new SelectListItem { Text = p.nombreProvincia, Value = p.idProvincia.ToString() });
            }

            ViewData["provincias"] = li;

            return View(listadoProvincias);
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

        [HttpPost]
        public ActionResult RegistrarUsuario(UserModel user)
        {
            if (ModelState.IsValid) //Verificar que el modelo de datos sea válido en cuanto a la definición de las propiedades
            {
                if (IsUserValid(user.mail))//Verificar si el usuario(email) existe utilizando el método privado
                {
                    //El usuario no existe
                    var bd = new bdVentaLibrosDataContext();

                    Usuarios nuevoUsuario = null;
                    if (user.idProvincia > 0)
                    {
                        //Convierto el model a una entidad de dominio comentario
                        nuevoUsuario = new Usuarios
                        {
                            //idUsuario = user.idUsuario,
                            mail = user.mail,
                            contraseña = user.contraseña,
                            nombreUsuario = user.nombreUsuario,
                            direccion = user.direccion,
                            idProvincia = user.idProvincia,
                            idLocalidad = user.idLocalidad,
                            idPerfil = 2,
                            fechaHoraAlta = DateTime.Now
                        };
                    }
                    else
                    {
                        nuevoUsuario = new Usuarios
                        {
                            //idUsuario = user.idUsuario,
                            mail = user.mail,
                            contraseña = user.contraseña,
                            nombreUsuario = user.nombreUsuario,
                            direccion = user.direccion,
                            idPerfil = 2,
                            fechaHoraAlta = DateTime.Now
                        };
                    }              

                    //Agregando un nuevo registro 
                    bd.Usuarios.InsertOnSubmit(nuevoUsuario);

                    //Hacer el submit
                    bd.SubmitChanges();

                    WebSecurity.CreateUserAndAccount(nuevoUsuario.mail, nuevoUsuario.contraseña);
                    Roles.AddUserToRole(nuevoUsuario.mail, "Cliente");

                    if (nuevoUsuario.idPerfil == 1)
                    {
                        WebMatrix.WebData.WebSecurity.Login("admin@admin.com", user.contraseña);
                    }
                    else
                    {
                        WebMatrix.WebData.WebSecurity.Login(nuevoUsuario.mail, nuevoUsuario.contraseña);
                    }
                    System.Web.HttpContext.Current.Session["IDUSUARIO"] = nuevoUsuario.idUsuario;
                    System.Web.HttpContext.Current.Session["nombreUsuario"] = nuevoUsuario.nombreUsuario;
                    FormsAuthentication.SetAuthCookie(nuevoUsuario.mail, false); //crea variable de user con el usuario
                    return RedirectToAction("Index", "Home");

                    //WebSecurity.CreateUserAndAccount(nuevoUsuario.nombreUsuario, nuevoUsuario.contraseña);
                    //Roles.AddUserToRole(nuevoUsuario.nombreUsuario, "Cliente");
                    //FormsAuthentication.SetAuthCookie(user.mail, false); //crea variable de user con el usuario
                    //return RedirectToAction("Index", "Home"); //dirigir al controlador home vista Index una vez se a autenticado en el sistema
                }
                else
                {
                    //El usuario ya existe
                    ModelState.AddModelError("", "Ya existe un usuario con ese email");

                }
            }

            return View();
        }

        // Metodo para validar el usuario y password del usuario, realiza la consulta a la base de datos
        private bool Isvalid(string mail, string contraseña)
        {
            bool Isvalid = false;
            using (var bd = new bdVentaLibrosDataContext())
            {
                var user = bd.Usuarios.FirstOrDefault(u => u.mail == mail);
                if (user != null)
                {
                    if (user.contraseña == contraseña) //Verificar contraseña del usuario
                    {
                        Isvalid = true;
                    }
                }
            }
            return Isvalid;
        }

        // Metodo para validar el usuario (email), realiza la consulta a la base de datos
        private bool IsUserValid(string mail)
        {
            bool Isvalid = true;
            using (var bd = new bdVentaLibrosDataContext())
            {
                var user = bd.Usuarios.FirstOrDefault(u => u.mail == mail);
                if (user != null)
                {
                    Isvalid = false;
                }
            }
            return Isvalid;
        }
    }
}

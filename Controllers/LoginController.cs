using DataModels;
using JCO_ProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace JCO_ProyectoFinal.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // Se cierra o abandona la sesion
        public ActionResult Logout()
        {
            
            Session.Abandon();
            FormsAuthentication.SignOut();

            
            return RedirectToAction("Login", "Login");
        }

        
        // Se valida el usuario y se obtienen datos como el id usuario, tipo persona y nombre

        [HttpPost]
        public ActionResult Login(string email, string clave)
        {
            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    
                    var usuario = db.SpValidarUsuario(email, clave).FirstOrDefault();

                   
                    if (usuario != null && usuario.Estado.HasValue && usuario.Estado.Value)
                    {
                       
                        Session["UsuarioLogeado"] = usuario.Id_persona;
                        Session["TipoPersona"] = usuario.Tipo_persona;
                        Session["UsuarioNombre"] = usuario.Nombre;

                       
                        if (usuario.Tipo_persona == "Empleado")
                        {
                            return RedirectToAction("Index", "Casas");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Cobros");
                        }
                    }
                    else
                    {
                        ViewBag.Mensaje = usuario == null ? "Credenciales Inválidas" : "Usuario Inactivo";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error: No se pudieron ingresar los datos";
                Debug.WriteLine($"Error: {ex.Message}");
            }

            return View();
        }
    }

}
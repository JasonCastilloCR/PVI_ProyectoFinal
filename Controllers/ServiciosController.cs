using DataModels;
using JCO_ProyectoFinal.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DataModels.PviProyectoFinalDBStoredProcedures;

namespace JCO_ProyectoFinal.Controllers
{
    public class ServiciosController : Controller
    {
        // GET: Servicios
        public ActionResult Index()
        {

            // Verifica que el usuario esté logueado
            var usuarioLogeadoId = Session["UsuarioLogeado"] as int?;
            if (!usuarioLogeadoId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

          
            var tipoPersona = Session["TipoPersona"] as string;

          
            if (string.IsNullOrEmpty(tipoPersona))
            {
              
                return RedirectToAction("Login", "Login");
            }

            Debug.WriteLine($"Tipo Persona: {tipoPersona}");

            ViewBag.TipoPersona = tipoPersona;

           


            return View();
        }


        public JsonResult ListaServicios()
        {
            
            var servicios = new List<SpGestionarServiciosResult>();
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                servicios = db.SpGestionarServicios().ToList();
            }

            Debug.WriteLine(JsonConvert.SerializeObject(servicios));
            return Json(new { data = servicios }, JsonRequestBehavior.AllowGet);
        }


        // GET: CrearServicio
        public ActionResult CrearServicio(int? id)
        {
            var usuarioLogeadoId = Session["UsuarioLogeado"] as int?;
            if (!usuarioLogeadoId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            var servicio = new ModelServicio();
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                if (id.HasValue)
                {
                    servicio = db.SpGestionarServicios()
                             .Where(s => s.Id_servicio == id.Value)
                             .Select(s => new ModelServicio
                             {
                                 Id = s.Id_servicio,
                                 IdCategoria = s.Id_categoria,
                                 NombreServicio = s.Nombre_servicio,
                                 Precio = s.Precio,
                                 nombreCategoria = s.Nombre_categoria,
                                 Descripcion = s.Descripcion,
                                 Estado = s.Estado_servicio
                             }).FirstOrDefault();
                }


                ViewBag.Categorias = db.SpRetornaCategorias().ToList();
            }
            return View(servicio);
        }

        [HttpPost]
        public ActionResult CrearServicio(ModelServicio servicio)
        {
            string resultado;
            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    
                    if (servicio.Id == 0)
                    {
                    db.SpCrearServicio(servicio.NombreServicio, servicio.Descripcion, servicio.Precio, servicio.IdCategoria, servicio.Estado == "Activo");
                    }
                    else
                    {
                        db.SpModServicio(servicio.Id, servicio.Descripcion, servicio.Precio);
                    }


                    ViewBag.Categorias = db.SpRetornaCategorias().ToList();
                    resultado = "Servicio guardado exitosamente.";
                }
            }
            catch (Exception)
            {
                resultado = "Error al guardar Servicio.";
            }

            ViewBag.Resultado = resultado;
            return View(servicio);
        }



        [HttpPost]
        public JsonResult Inactivar(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "ID inválido." });
            }

            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    db.SpInactivarServicio(id); 
                }

                return Json(new { success = true, message = "Servicio inactivado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

    }
}
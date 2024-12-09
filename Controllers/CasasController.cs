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
    public class CasasController : Controller
    {


       
        // GET: Casas


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

        // Se listan las casas por medio de formato JSON para listarlas en un datatable
        public JsonResult ListaCasas()
        {
            
            
            
            var casas = new List<SpGestionarCasasResult>();
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                casas = db.SpGestionarCasas().ToList();
            }

            Debug.WriteLine(JsonConvert.SerializeObject(casas));
            return Json(new { data = casas}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CrearCasa(int id)
        {
            try
            {
                using (var db = new PviProyectoFinalDB())
                {
                    // Obtén los datos de la casa
                    var casa = db.Casas.FirstOrDefault(c => c.IdCasa == id);
                    if (casa == null)
                    {
                        TempData["ErrorMessage"] = "La casa no existe.";
                        return RedirectToAction("Index");
                    }

                    // Validación 1: Si la casa está inactiva
                    if (!casa.Estado)
                    {
                        
                        return RedirectToAction("Index" , "Casas");
                    }

                    // Validación 2: Si la casa tiene cobros pendientes
                    var CobrosPendientes = db.Cobros.Any(c => c.IdCasa == id && c.Estado == "Pendiente");
                    if (CobrosPendientes)
                    {
                        
                        return RedirectToAction("Index" , "Casas");
                    }

                    // Mapea los datos de la casa al modelo
                    var model = new ModelCasa
                    {
                        Id = casa.IdCasa,
                        NombreCasa = casa.NombreCasa,
                        MetrosCuadrados = casa.MetrosCuadrados,
                        NumeroHabitaciones = casa.NumeroHabitaciones,
                        NumeroBanos = casa.NumeroBanos,
                        Precio = (int)casa.Precio,
                        IdPersona = casa.IdPersona,
                        Propietario = db.Personas.Where(p => p.IdPersona == casa.IdPersona)
                                                 .Select(p => p.Nombre + " " + p.Apellido).FirstOrDefault(),
                        FechaConstruccion = casa.FechaConstruccion,
                        Estado = casa.Estado
                    };

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: CrearCasa

        // Se obtienen los datos para utilizarlos en editar y crear casa
        //public ActionResult CrearCasa(int? id)
        //{
        //    var usuarioLogeadoId = Session["UsuarioLogeado"] as int?;
        //    if (!usuarioLogeadoId.HasValue)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }

        //    var casa = new ModelCasa();
        //    using (var db = new PviProyectoFinalDB("MyDatabase"))
        //    {
                
        //        if (id.HasValue)
        //        {
        //            casa = db.SpGestionarCasas()
        //                     .Where(c => c.Id_casa == id.Value)
        //                     .Select(c => new ModelCasa
        //                     {
        //                         Id = c.Id_casa,
        //                         NombreCasa = c.Nombre_casa,
        //                         MetrosCuadrados = c.Metros_cuadrados,
        //                         NumeroHabitaciones = c.Numero_habitaciones,
        //                         NumeroBanos = c.Numero_banos,
        //                         IdPersona = c.Id_persona,
        //                         Propietario = c.Propietario,
        //                         FechaConstruccion = c.Fecha_construccion,
        //                         Estado = c.Estado 
        //                     }).FirstOrDefault();
        //        }

                
        //        ViewBag.Personas = db.RetornaPersonas().ToList(); 
        //    }
        //    return View(casa);
        //}

        // Se realizan las condicionales para saber si se debe crear o editar la casa y se ejecutan por medio de un POST

        [HttpPost]
        public ActionResult CrearCasa(ModelCasa casa)
        {
            string resultado;
            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    
                    if (casa.Id == 0)
                    {
                        db.SpCrearCasas(casa.NombreCasa, casa.MetrosCuadrados, casa.NumeroHabitaciones, casa.NumeroBanos, casa.IdPersona, casa.FechaConstruccion, casa.Estado = true);
                    }
                    else
                    {
                        db.SpModCasas(casa.Id, casa.NumeroHabitaciones, casa.NumeroBanos);
                    }


                    ViewBag.Personas = db.RetornaPersonas().ToList();
                    resultado = "La casa se ha guardado exitosamente.";

                }


            }
            catch (Exception)
            {
                resultado = "Error al guardar la casa.";
            }

            ViewBag.Resultado = resultado;
            return View(casa);
        }

     


        // Se inactivan las casas por medio de formato JSON

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
                    db.SpInactivarCasa(id); // Llama al procedimiento almacenado
                }

                return Json(new { success = true, message = "Casa inactivada exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

    }
}
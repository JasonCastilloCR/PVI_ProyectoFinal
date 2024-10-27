using DataModels;
using JCO_ProyectoFinal.Models;
using System;
using System.Collections.Generic;
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
            var casas = new List<Casa>();
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                // se obtiene la lista de casas recorriendo el sp
                casas = db.SpGestionarCasas().ToList();
            }
            return View(casas);
        }

        // GET: CrearCasa
        public ActionResult CrearCasa(int? id)
        {
            var casa = new ModelCasa();
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                // Si existe un ID, se obtiene la información de la casa para editar
                if (id.HasValue)
                {
                    casa = db.SpGestionarCasas()
                             .Where(c => c.IdCasa == id.Value)
                             .Select(c => new ModelCasa
                             {
                                 Id = c.IdCasa,
                                 NombreCasa = c.NombreCasa,
                                 MetrosCuadrados = c.MetrosCuadrados,
                                 NumeroHabitaciones = c.NumeroHabitaciones,
                                 NumeroBanos = c.NumeroBanos,
                                 IdPersona = c.IdPersona,
                                 FechaConstruccion = c.FechaConstruccion,
                                 Estado = c.Estado == true  // Se asigna como activo o inactivo
                             }).FirstOrDefault();
                }

                
                //ViewBag.Personas = db.RetornaPersonas().ToList(); pendiente el sp para el dropdown
            }
            return View(casa);
        }

        [HttpPost]
        public ActionResult CrearCasa(ModelCasa casa)
        {
            string resultado;
            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    // Si el ID es 0, se crea una nueva casa usando spCrearCasas, si no, se actualiza usando spModCasas
                    if (casa.Id == 0)
                    {
                        db.SpCrearCasas(casa.NombreCasa, casa.MetrosCuadrados, casa.NumeroHabitaciones, casa.NumeroBanos, casa.IdPersona, casa.FechaConstruccion, casa.Estado ? true : false);
                    }
                    else
                    {
                        db.SpModCasas(casa.Id, casa.NombreCasa, casa.MetrosCuadrados, casa.NumeroHabitaciones, casa.NumeroBanos, casa.IdPersona, casa.FechaConstruccion, casa.Estado ? true : false);
                    }

                    
                    //ViewBag.Personas = db.RetornaPersonas().ToList();
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

        // GET: Eliminar Casa
        public ActionResult Eliminar(int? id)
        {
            var casas = new List<Casa>();
            using (var db = new PviProyectoFinalDB("MyDatabase"))
            {
                db.SpModCasas(id, null, 0, 0, 0, 0, DateTime.Now, false);  //En proceso spEliminarCasas
                casas = db.SpGestionarCasas().ToList();
            }
            return View("Index", casas);
        }
    }
}
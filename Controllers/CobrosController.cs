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
    public class CobrosController : Controller
    {
        // GET: Cobros
        public ActionResult Index()
        {
            var usuarioLogeadoId = Session["UsuarioLogeado"] as int?;

            if (usuarioLogeadoId.HasValue) { 
                var cobros = new List<SpGestionarCobrosResult>();

            using (var db = new PviProyectoFinalDB())
            {
                
                cobros = db.SpGestionarCobros().ToList();
            }

            return View(cobros);
            } else
            {
                return RedirectToAction("Login", "Login");
            }
        }
    }
}
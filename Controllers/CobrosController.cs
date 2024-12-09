using DataModels;
using JCO_ProyectoFinal.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static DataModels.PviProyectoFinalDBStoredProcedures;

namespace JCO_ProyectoFinal.Controllers
{
    public class CobrosController : Controller
    {
        // GET: Cobros
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

        //Lista los cobros por medio de formato JSON
        public JsonResult ListaCobros()
        {
            var cobros = new List<ModelCobros>();

            try
            {
                var tipoPersona = Session["TipoPersona"] as string;
                var idPersona = Session["UsuarioLogeado"] as int?;

                if (string.IsNullOrEmpty(tipoPersona))
                {
                    throw new Exception("El tipo de persona no está definido en la sesión.");
                }

                if (!idPersona.HasValue)
                {
                    throw new Exception("El ID del usuario no está definido en la sesión.");
                }

                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    if (tipoPersona == "Empleado")
                    {
                        cobros = db.SpGestionarCobros()
                                   .Select(c => new ModelCobros
                                   {
                                       IdCobro = c.Id_cobro,
                                       Cliente = c.Cliente,
                                       Casa = c.Casa,
                                       Periodo = c.Periodo,
                                       Estado = c.Estado,
                                       Monto = c.Monto,
                                       FechaPagada = c.FechaPagada
                                   })
                                   .ToList();
                    }
                    else if (tipoPersona == "Cliente")
                    {
                        cobros = db.SpMisCobros(idPersona.Value)
                                   .Select(c => new ModelCobros
                                   {
                                       IdCobro = c.Id_cobro,
                                       Cliente = c.Cliente,
                                       Casa = c.Casa,
                                       Periodo = c.Periodo,
                                       Estado = c.Estado,
                                       Monto = c.Monto,
                                       FechaPagada = c.FechaPagada
                                   })
                                   .ToList();
                    }
                    else
                    {
                        throw new Exception("Tipo de persona no válido.");
                    }
                }

                Debug.WriteLine(JsonConvert.SerializeObject(cobros));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, data = cobros }, JsonRequestBehavior.AllowGet);
        }


        // Se obtienen los datos para listar detalle cobros y bitacora
        public ActionResult DetalleCobros(int id_cobro)
        {
            var usuarioLogeadoId = Session["UsuarioLogeado"] as int?;
            if (!usuarioLogeadoId.HasValue)
            {


                if (Request.IsAjaxRequest())
                {
                    return Json(new { redirectTo = Url.Action("Login", "Login") }, JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Login", "Login");
            }
            var tipoPersona = Session["TipoPersona"] as string;
            var idPersona = Session["UsuarioLogeado"] as int?;
            ViewBag.TipoPersona = tipoPersona;
            ViewBag.IdPersona = idPersona;

            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    var rcobro = db.SpDetalleCobro(id_cobro).FirstOrDefault();


                    if (rcobro == null)
                    {
                        return new HttpNotFoundResult("No se encontraron detalles para el cobro especificado.");
                    }

                    var detallesCobro = new DetalleCobroModel
                    {
                        IdCobro = rcobro.Id_cobro,
                        Propietario = rcobro.Propietario,
                        NombreCasa = rcobro.Nombre_casa,
                        Monto = rcobro.Monto,
                        PrecioCasa = rcobro.Precio_casa.HasValue ? (int)rcobro.Precio_casa : 0,
                        Periodo = rcobro.Periodo,
                        Estado = rcobro.Estado,
                        // To do: Agregar elemento de bitacora
                    };

                    var serviciosJson = rcobro.Servicios;
                    Debug.WriteLine($"serviciosJson: {serviciosJson}");
                    if (!string.IsNullOrEmpty(serviciosJson))
                    {
                        detallesCobro.Servicios = JsonConvert.DeserializeObject<List<ServiciosCobros>>(serviciosJson);
                        foreach (var servicio in detallesCobro.Servicios)
                        {
                            Debug.WriteLine($"ID: {servicio.id_servicio}, Nombre: {servicio.nombre_servicio}, Adquirido: {servicio.Adquirido}");
                        }

                    }
                    else
                    {
                        detallesCobro.Servicios = new List<ServiciosCobros>();
                    }


                    var BitacoraJson = rcobro.Bitacora;
                    Debug.WriteLine($"BitacoraJson: {BitacoraJson}");
                    if (!string.IsNullOrEmpty(BitacoraJson))
                    {
                        detallesCobro.Bitacora = JsonConvert.DeserializeObject<List<ListarBitacora>>(BitacoraJson);
                        foreach (var bitacora in detallesCobro.Bitacora)
                        {
                            Debug.WriteLine($"ID: {bitacora.id_bitacora}, Nombre: {bitacora.nombre_usuario}, Fecha: {bitacora.fecha_bitacora}, Accion: {bitacora.detalle_bitacora}");
                        }

                    }
                    else
                    {
                        detallesCobro.Servicios = new List<ServiciosCobros>();
                    }





                    return PartialView("DetalleCobros", detallesCobro);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en DetalleCobros: {ex.Message}");
                return new HttpStatusCodeResult(500, "Ocurrió un error inesperado al obtener los detalles del cobro.");
            }
        }


        // GET: CrearCobro
        public ActionResult CrearCobro(int? idPersona, int? idCobro)
        {

            var usuarioLogeadoId = Session["UsuarioLogeado"] as int?;
            if (!usuarioLogeadoId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }
            var tipoPersona = Session["TipoPersona"] as string;
            ViewBag.TipoPersona = tipoPersona;

            var model = new CrearCobroModel();
            var modelEditar = new DetalleCobroModel();


            using (var db = new PviProyectoFinalDB())
            {
                model.Clientes = db.SpRetornaPersona()
                                   .Where(c => c.Estado_persona == "Activo")  
                                   .Select(c => new SelectListItem
                                   {
                                       Value = c.Id_persona.ToString(),
                                       Text = c.Nombre
                                   }).ToList();

                if (idPersona.HasValue)
                {
                    model.Casas = db.SpRetornaCasa(idPersona.Value)
                                   .Select(c => new CasaViewModel
                                   {
                                       IdCasa = c.Id_casa,
                                       NombreCasa = c.Nombre_casa
                                   }).ToList();
                }


                model.Anios = Enumerable.Range(2024, 11)
                                        .Select(a => new SelectListItem
                                        {
                                            Value = a.ToString(),
                                            Text = a.ToString()
                                        })
                                        .ToList();

                model.Meses = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Seleccione un mes" },
                new SelectListItem { Value = "1", Text = "Enero" },
                new SelectListItem { Value = "2", Text = "Febrero" },
                new SelectListItem { Value = "3", Text = "Marzo" },
                new SelectListItem { Value = "4", Text = "Abril" },
                new SelectListItem { Value = "5", Text = "Mayo" },
                new SelectListItem { Value = "6", Text = "Junio" },
                new SelectListItem { Value = "7", Text = "Julio" },
                new SelectListItem { Value = "8", Text = "Agosto" },
                new SelectListItem { Value = "9", Text = "Septiembre" },
                new SelectListItem { Value = "10", Text = "Octubre" },
                new SelectListItem { Value = "11", Text = "Noviembre" },
                new SelectListItem { Value = "12", Text = "Diciembre" }
            };

                if (!idPersona.HasValue)
                {
                    model.ListaServicios = db.SpGestionarServicios()
                                       .Where(s => s.Estado_servicio == "Activo")
                                       .Select(s => new ServicioViewModel
                                       {
                                           IdServicio = s.Id_servicio,
                                           NombreServicio = s.Nombre_servicio,

                                       }).ToList();
                }


                return View(model);
            }
        }



        [HttpPost]
        public JsonResult GetCasasByCliente(int clienteId)
        {
            using (var db = new PviProyectoFinalDB())
            {
                var casas = db.SpRetornaCasa(clienteId)
                              .Select(c => new
                              {
                                  c.Id_casa,
                                  c.Nombre_casa
                              })
                              .ToList();

                return Json(casas);
            }

        }

        [HttpPost]
        public ActionResult CrearCobro(CrearCobroModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using (var db = new PviProyectoFinalDB())
                {


                    var idCobro = db.SpCrearCobro(
                        model.IdCasa,
                        model.Anno,
                        model.Mes,
                        model.Monto
                    ).FirstOrDefault();

                    if (idCobro?.Id_cobro != 0)
                    {

                        foreach (var servicioId in model.ServiciosSeleccionados)
                        {
                            db.SpInsertarDetalleCobro((int?)idCobro.Id_cobro, servicioId);
                        }

                        db.SpCalcularCobro((int?)idCobro.Id_cobro, model.IdCasa);

                        db.SpBitacora(DateTime.Now, "CREADO", (int?)idCobro.Id_cobro, model.ClienteId);


                    }


                    return RedirectToAction("Index", "Cobros");
                }
            }
            catch (Exception ex)
            {

                ViewBag.ErrorMessage = "Ocurrió un error al guardar el cobro: " + ex.Message;
                return View(model);
            }
        }


        //GET
        public ActionResult EditarCobro(int? idPersona, int? idCobro, int? idCasa)
        {
            var usuarioLogeadoId = Session["UsuarioLogeado"] as int?;
            if (!usuarioLogeadoId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }
            var tipoPersona = Session["TipoPersona"] as string;
            ViewBag.TipoPersona = tipoPersona;

            var modelEditar = new EditarCobroModel();
  


            using (var db = new PviProyectoFinalDB())
            {
               

                if (!idPersona.HasValue)
                {
                    modelEditar.EdicionServicios = db.SpGestionarServicios()
                                       .Where(s => s.Estado_servicio == "Activo")
                                       .Select(s => new ServicioModel
                                       {
                                           id_servicio = s.Id_servicio,
                                           nombre_servicio = s.Nombre_servicio,

                                       }).ToList();
                }

                var rservicios = db.SpDetalleCobro(idCobro).FirstOrDefault();
                if (idCobro.HasValue)
                {
                    modelEditar = db.SpDetalleCobro(idCobro)
               .Select(c => new EditarCobroModel
               {

                   idCobro = c.Id_cobro,
                   IdCasa = c.Id_casa,
                   Propietario = c.Propietario,
                   nombreCasa = c.Nombre_casa,
                   Periodo = c.Periodo,
                   Estado = c.Estado,
                   Monto = c.Monto
                   //To do: Agregar elemento servicios
               }).FirstOrDefault();


                    var editserviciosJson = rservicios.Servicios;
                    Debug.WriteLine($"serviciosJson: {editserviciosJson}");
                    if (!string.IsNullOrEmpty(editserviciosJson))
                    {
                        modelEditar.EdicionServicios = JsonConvert.DeserializeObject<List<ServicioModel>>(editserviciosJson);
                        foreach (var servicio in modelEditar.EdicionServicios)
                        {
                            Debug.WriteLine($"ID: {servicio.id_servicio}, Nombre: {servicio.nombre_servicio}, Adquirido: {servicio.Adquirido}");
                        }

                    }
                    else
                    {
                        modelEditar.EdicionServicios = new List<ServicioModel>();
                    }
                }
            }
            return View(modelEditar);
        }

        [HttpPost]
        public ActionResult EditarCobro(EditarCobroModel modelEditar, int idCobro, int idCasa)
        {
            var usuarioLogeadoId = Session["UsuarioLogeado"] as int?;
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Los datos proporcionados no son válidos. Por favor, revisa el formulario.";
                return View(modelEditar);
            }

            try
            {
                using (var db = new PviProyectoFinalDB())
                {
                    Debug.WriteLine($"Editando id casa: {idCasa}");

                    // Verifica que el ID del cobro sea válido
                    if (idCobro != 0)
                    {
                        Debug.WriteLine($"Antes de eliminar servicios del cobro: {idCobro}");

                        
                        db.SpEliminarDetalleCobro(idCobro);

                        Debug.WriteLine($"Después de eliminar servicios del cobro: {idCobro}");

                        
                        if (modelEditar.ServiciosSeleccionados != null && modelEditar.ServiciosSeleccionados.Any())
                        {
                            foreach (var servicioId in modelEditar.ServiciosSeleccionados)
                            {
                                db.SpInsertarDetalleCobro(idCobro, servicioId);
                                Debug.WriteLine($"Insertado: idCobro={idCobro}, servicioID={servicioId}");
                            }
                        }

                     
                        db.SpCalcularCobro(idCobro, idCasa);

                     
                        db.SpBitacora(DateTime.Now, "MODIFICADO", idCobro, usuarioLogeadoId);
                    }
                    else
                    {
                        Debug.WriteLine($"No se proporcionó un ID válido para la casa: {modelEditar.IdCasa}");
                        ViewBag.ErrorMessage = "El ID del cobro no es válido.";
                        return View(modelEditar);
                    }
                }

                TempData["SuccessMessage"] = "El cobro se ha editado correctamente.";
                return RedirectToAction("Index", "Cobros");
            }
            catch (Exception ex)
            {
              
                Debug.WriteLine($"Error al editar el cobro: {ex.Message}");
                ViewBag.ErrorMessage = "Ocurrió un error al editar el cobro: " + ex.Message;
                return View(modelEditar);
            }
        }

        [HttpPost]
        public JsonResult EliminarCobro(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "ID inválido." });
            }

            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {
                    db.SpEliminarDetalleCobro(id);
                    db.SpEliminarCobro(id); 
                }

                return Json(new { success = true, message = "Cobro eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public JsonResult PagarCobro(int? id)
        {
            var usuarioLogeadoId = Session["UsuarioLogeado"] as int?;
            if (id == null)
            {
                return Json(new { success = false, message = "ID inválido." });
            }

            try
            {
                using (var db = new PviProyectoFinalDB("MyDatabase"))
                {

                    db.SpPagarCobro(id);
                    db.SpBitacora(DateTime.Now, "PAGADO", id, usuarioLogeadoId);
                }

                return Json(new { success = true, message = "Cobro pagado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

    }
}
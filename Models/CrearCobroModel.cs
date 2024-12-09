using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DataModels.PviProyectoFinalDBStoredProcedures;

namespace JCO_ProyectoFinal.Models
{
    public class CrearCobroModel : SpGestionarCobrosResult
    {
        public int idCobro {  get; set; }
        public int ClienteId { get; set; } 
        public string Cliente { get; set; }
        public int IdCasa { get; set; }    
        public int Anno { get; set; }       
        public int Mes { get; set; }
        public decimal Monto { get; set; }
        public List<SelectListItem> Clientes { get; set; } = new List<SelectListItem>(); 
        public List<CasaViewModel> Casas { get; set; } = new List<CasaViewModel>(); 

        public List<SelectListItem> Anios { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Meses { get; set; } = new List<SelectListItem>();

        // Lista de Servicios
        public List<ServicioViewModel> ListaServicios { get; set; } = new List<ServicioViewModel>();

        // Servicios seleccionados (para enviar al procedimiento)
        public List<int> ServiciosSeleccionados { get; set; } = new List<int>();

       
    }

    public class CasaViewModel : SpRetornaCasaResult
    {
        public int IdCasa { get; set; }
        public string NombreCasa { get; set; }


    }

    public class ServicioViewModel : SpGestionarServiciosResult
    {
        public int IdServicio { get; set; }
        public string NombreServicio { get; set; }

        public decimal Precio { get; set; }
    }

    
}
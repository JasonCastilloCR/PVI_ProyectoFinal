using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DataModels.PviProyectoFinalDBStoredProcedures;
using System.Web.Mvc;

namespace JCO_ProyectoFinal.Models
{
    public class EditarCobroModel : SpDetalleCobroResult
    {
        public int idCobro { get; set; }
        public int ClienteId { get; set; }
        public int IdCasa { get; set; }
        public string nombreCasa { get; set; }
        public string Periodo { get; set; }
        //public decimal Monto { get; set; }
        public string Propietario { get; set; }
        public List<ServicioModel> EdicionServicios { get; set; } = new List<ServicioModel>();
        public List<int> ServiciosSeleccionados { get; set; } = new List<int>();
    }

    public class CasaModel : SpRetornaCasaResult
    {
        public int IdCasa { get; set; }
        public string NombreCasa { get; set; }


    }

    public class ServicioModel
    {
        public int id_servicio { get; set; }
        public string nombre_servicio { get; set; }

        public int Adquirido { get; set; }
    }
}
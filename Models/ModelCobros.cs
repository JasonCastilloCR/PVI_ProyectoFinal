using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DataModels.PviProyectoFinalDBStoredProcedures;

namespace JCO_ProyectoFinal.Models
{
    public class ModelCobros : SpGestionarCobrosResult
    {
        public int IdCobro { get; set; }
        public string Cliente { get; set; }
        public string Casa { get; set; }
        public string Periodo { get; set; }
        public string Estado { get; set; }
        public decimal Monto { get; set; }
        public DateTime? FechaPagada { get; set; }
    }

}
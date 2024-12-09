using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DataModels.PviProyectoFinalDBStoredProcedures;

namespace JCO_ProyectoFinal.Models
{


    public class DetalleCobroModel : SpDetalleCobroResult
    {
        public int IdCobro { get; set; }
        public string Propietario { get; set; }
        public string NombreCasa { get; set; }
        public decimal Monto { get; set; }
        public int PrecioCasa { get; set; }
        public string Periodo { get; set; }
        public string Estado { get; set; }

        // Propiedad para manejar el array de servicios directamente
        public List<ServiciosCobros> Servicios { get; set; } = new List<ServiciosCobros>();
        public List<ListarBitacora> Bitacora { get; set; } = new List<ListarBitacora>();
    }

    public class ServiciosCobros
    {
        public int id_servicio { get; set; }
        public string nombre_servicio { get; set; }
        public int Adquirido { get; set; } // 1 = Adquirido, 0 = No adquirido
    }

    public class ListarBitacora : Bitacora
    {
        public int id_bitacora { get; set; }

        public string detalle_bitacora { get; set; }
        public DateTime fecha_bitacora { get; set; }
 
        public string nombre_usuario { get; set; }

    }
}
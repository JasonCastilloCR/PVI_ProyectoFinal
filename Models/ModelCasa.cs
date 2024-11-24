using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JCO_ProyectoFinal.Models
{
    public class ModelCasa : Casa
    {

       
        public int Id { get; set; }
        public string NombreCasa { get; set; }
        public int MetrosCuadrados { get; set; }
        public int NumeroHabitaciones { get; set; }
        public int NumeroBanos { get; set; }
        public int IdPersona { get; set; }
        public DateTime FechaConstruccion { get; set; }
        public bool Estado { get; set; }  

    }
}
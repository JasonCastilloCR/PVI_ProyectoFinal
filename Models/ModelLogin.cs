using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DataModels.PviProyectoFinalDBStoredProcedures;

namespace JCO_ProyectoFinal.Models
{
    public class ModelLogin : SpValidarUsuarioResult
    {
        public int IdPersona { get; set; }
        public bool TipoPersona { get; set; }
        public string estado { get; set; }
    }
}
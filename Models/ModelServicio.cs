using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JCO_ProyectoFinal.Models
{
    public class ModelServicio : Servicio
    {
            public int Id { get; set; }

            public int IdCategoria { get; set; }
            public string NombreServicio { get; set; }
            public string Descripcion { get; set; }
            public decimal Precio { get; set; }
            public string nombreCategoria { get; set; }
            public string Estado { get; set; }

    }
}
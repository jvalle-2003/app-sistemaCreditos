using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace app_sistemaCreditos.Models.usuarios
{
    public class Usuarios   
    {
        public int ID { get; set; }
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        public int IdEmpleado { get; set; }
    }
}
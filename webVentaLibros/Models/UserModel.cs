﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace webVentaLibros.Models
{
    public class UserModel
    {

        public string mail { get; set; }

        [DataType(DataType.Password)]
        public string contraseña { get; set; }

        public string nombreUsuario { get; set; }
        public string direccion { get; set; }
        public int idUsuario { get; set; }
        public int idProvincia { get; set; }
        public int idLocalidad { get; set; }
        public string fechaHoraAlta { get; set; }
        public string fechaHoraBaja { get; set; }
        public int idPerfil { get; set; }
    }
}
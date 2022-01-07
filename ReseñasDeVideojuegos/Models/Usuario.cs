using System;
using System.Collections.Generic;

#nullable disable

namespace ReseñasDeVideojuegos.Models
{
    public partial class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombreusuario { get; set; }
        public string Contrasena { get; set; }
        public ulong Estado { get; set; }
    }
}

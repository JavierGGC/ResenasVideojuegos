using System;
using System.Collections.Generic;

#nullable disable

namespace ReseñasVideojuegos.Models
{
    public partial class Administrador
    {
        public int IdAdmin { get; set; }
        public string Nombreadmin { get; set; }
        public string Contrasena { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace ReseñasVideojuegos.Models
{
    public partial class Resena
    {
        public int IdResenas { get; set; }
        public string Titulo { get; set; }
        public string Compañia { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public ulong Estado { get; set; }
    }
}

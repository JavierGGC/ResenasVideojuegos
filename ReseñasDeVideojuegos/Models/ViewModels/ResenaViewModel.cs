using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReseñasDeVideojuegos.Models.ViewModels
{
    public class ResenaViewModel
    {
        public Resena ResenaJuego { get; set; }
        public IFormFile Archivo { get; set; }
        public string Imagen { get; set; }
    }
}

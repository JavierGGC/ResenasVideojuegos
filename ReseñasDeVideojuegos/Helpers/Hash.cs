using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReseñasDeVideojuegos.Helpers
{
    public static class Hash
    {
        public static string GetHash(string cadena)
        {
            var alg = SHA512.Create();
            var array = Encoding.UTF8.GetBytes(cadena);
            var hash = alg.ComputeHash(array).Select(x => x.ToString("x2"));
            return string.Join("", hash);
        }
    }
}

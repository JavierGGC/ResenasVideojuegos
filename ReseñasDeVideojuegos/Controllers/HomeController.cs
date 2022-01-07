using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReseñasDeVideojuegos.Models;
using ReseñasDeVideojuegos.Models.ViewModels;
using ReseñasDeVideojuegos.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace ReseñasDeVideojuegos.Controllers
{
    public class HomeController : Controller
    {
        public resenajuegosContext Context { get; }
        public HomeController(resenajuegosContext rj)
        {
            Context = rj;
        }

        [Authorize(Roles = "Usuario, Admin")]
        public IActionResult Index()
        {
            IEnumerable<Resena> r = Context.Resenas.OrderBy(x => x.Titulo);
            return View(r);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoginUsuario()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUsuario(Usuario usu)
        {
            try
            {
                var usuario = await Context.Usuarios.SingleOrDefaultAsync(x => x.Nombreusuario == usu.Nombreusuario);
                if (usuario != null && Hash.GetHash(usu.Contrasena) == usuario.Contrasena)
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, usuario.Nombreusuario));
                    claims.Add(new Claim(ClaimTypes.Role, "Usuario"));
                    claims.Add(new Claim("Id", usu.IdUsuario.ToString()));
                    var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(new ClaimsPrincipal(identidad));
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Los datos ingresados son incorrectos, intente nuevamente.");
                    return View(usu);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("!", ex.Message);
                return View(usu);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(Usuario usu)
        {
            if (string.IsNullOrWhiteSpace(usu.Nombreusuario))
            {
                ModelState.AddModelError("", "Es necesario escribir un nombre de usuario.");
                return View(usu);
            }
            if (string.IsNullOrWhiteSpace(usu.Contrasena))
            {
                ModelState.AddModelError("", "Es necesario escribir una contraseña.");
                return View(usu);
            }
            if (Context.Usuarios.Any(x => x.Nombreusuario == usu.Nombreusuario))
            {
                ModelState.AddModelError("", "Este nombre de usuario no esta disponible, intente con otro.");
                return View(usu);
            }
            usu.Contrasena = Hash.GetHash(usu.Contrasena);
            usu.Estado = 1;
            Context.Add(usu);
            Context.SaveChanges();
            return RedirectToAction("LoginUsuario");
        }

        [Authorize(Roles = "Usuario, Admin")]
        [HttpGet]
        public IActionResult VerResena(int id)
        {
            Resena r = Context.Resenas.FirstOrDefault(x => x.IdResenas == id);

            if (r == null)
            {
                return RedirectToAction("Index");
            }
            return View(r);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("LoginUsuario");
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}


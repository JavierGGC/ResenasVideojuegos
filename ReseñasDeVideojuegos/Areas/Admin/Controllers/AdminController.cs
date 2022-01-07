using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReseñasDeVideojuegos.Models;
using ReseñasDeVideojuegos.Models.ViewModels;
using ReseñasDeVideojuegos.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ReseñasDeVideojuegos.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        public IWebHostEnvironment Env { get; }
        public resenajuegosContext Context { get; }
        public AdminController(resenajuegosContext rj, IWebHostEnvironment env)
        {
            Env = env;
            Context = rj;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            IEnumerable<Resena> r = Context.Resenas.OrderBy(x => x.Titulo);
            return View(r);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoginAdmin()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAdmin(Administrador admin)
        {
            try
            {
                var administrador = await Context.Administradors.SingleOrDefaultAsync(x => x.Nombreadmin == admin.Nombreadmin);
                var encriptado = Hash.GetHash(admin.Contrasena);
                if (administrador != null && encriptado == administrador.Contrasena)
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, administrador.Nombreadmin));
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    claims.Add(new Claim("Id", admin.IdAdmin.ToString()));
                    var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(new ClaimsPrincipal(identidad));
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Los datos ingresados son incorrectos, intente nuevamente.");
                    return View(admin);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("!", ex.Message);
                return View(admin);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Agregar()
        {
            return View();
           
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Agregar(ResenaViewModel rs)
        {   if (Context.Resenas.Any(x => x.Titulo == rs.ResenaJuego.Titulo))
            {
                ModelState.AddModelError("", "Ya existe una resena con este Titulo");
                return View(rs);
            }
            
            if (string.IsNullOrWhiteSpace(rs.ResenaJuego.Titulo))
            {
                ModelState.AddModelError("", "El titulo no puede estar vacio");
                return View(rs);
            }

            if (rs.ResenaJuego.Titulo.Length > 50)
            {
                ModelState.AddModelError("", "El titulo no puede pasar de los 50 caracteres.");
                return View(rs);
            }
            if (rs.ResenaJuego.Juego.Length > 50)
            {
                ModelState.AddModelError("", "El nombre del juego no puede pasar de los 50 caracteres.");
                return View(rs);
            }
            if (rs.ResenaJuego.Compañia.Length > 30)
            {
                ModelState.AddModelError("", "El nombre de la compañía no puede pasar de los 30 caracteres.");
                return View(rs);
            }
            if (string.IsNullOrWhiteSpace(rs.ResenaJuego.Fecha.ToString()))
            {
                ModelState.AddModelError("", "La fecha de la resena esta vacia");
                return View(rs);
            }
            if (string.IsNullOrWhiteSpace(rs.ResenaJuego.Juego))
            {
                ModelState.AddModelError("", "El nombre del juego esta vacio");
                return View(rs);
            }
            if (string.IsNullOrWhiteSpace(rs.ResenaJuego.Compañia))
            {
                ModelState.AddModelError("", "La compañia del juego esta vacia");
                return View(rs);
            }
            if (string.IsNullOrWhiteSpace(rs.ResenaJuego.Descripcion))
            {
                ModelState.AddModelError("", "La descripcion de la reseña esta vacia");
                return View(rs);
            }
            if (rs.ResenaJuego.Calificacion <=0 || rs.ResenaJuego.Calificacion >5)
            {
                ModelState.AddModelError("", "La calificacion tiene que ser mayor a 0 y menor a 5");
               
                return View(rs);
            }
           
            rs.ResenaJuego.Estado = 1;
            Context.Add(rs.ResenaJuego);
            Context.SaveChanges();

            if (rs.Archivo != null)
            {
                if (rs.Archivo.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("", "Solo se permiten archivos de JPG");

                    return View(rs);
                }
                if (rs.Archivo.Length > 1024 * 1024 * 5)
                {
                    ModelState.AddModelError("", "No es permitido cargar archivos mayores a 5MB");
                    return View(rs);
                }

                FileStream fs = new FileStream(Env.WebRootPath + "/imgs_resenasjuegos/" + rs.ResenaJuego.IdResenas + ".jpg", FileMode.Create);
                rs.Archivo.CopyTo(fs);
                fs.Close();
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [Route("Admin/Admin/Editar/{id}")]
        public IActionResult Editar(int id)
        {
            var r = Context.Resenas.FirstOrDefault(x => x.IdResenas == id);
            ResenaViewModel vm = new ResenaViewModel();
            if (r == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            vm.ResenaJuego = Context.Resenas.FirstOrDefault(x => x.IdResenas == id);
            return View(vm);
        }

        [HttpPost("Admin/Admin/Editar/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Editar(ResenaViewModel rs)
        {
            var r = Context.Resenas.FirstOrDefault(x => x.IdResenas == rs.ResenaJuego.IdResenas);

            if (string.IsNullOrWhiteSpace(rs.ResenaJuego.Titulo))
            {
                ModelState.AddModelError("", "El titulo no puede estar vacio");
                return View(rs);
            }
            if (string.IsNullOrWhiteSpace(rs.ResenaJuego.Descripcion))
            {
                ModelState.AddModelError("", "La descripcion de la reseña esta vacia");
                return View(rs);
            }
            if (rs.ResenaJuego.Calificacion <= 0 || rs.ResenaJuego.Calificacion > 5)
            {
                ModelState.AddModelError("", "La calificacion tiene que ser mayor a 0 y menor a 5");

                return View(rs);
            }
            r.Descripcion = rs.ResenaJuego.Descripcion;
            r.Calificacion = rs.ResenaJuego.Calificacion;

            Context.Update(r);
            Context.SaveChanges();

            if (rs.Archivo != null)
            {
                if (rs.Archivo.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("", "Solo se permiten archivos de JPG");

                    return View(rs);
                }
                if (rs.Archivo.Length > 1024 * 1024 * 5)
                {
                    ModelState.AddModelError("", "No es permitido cargar archivos mayores a 5MB");
                    return View(rs);
                }

                FileStream fs = new FileStream(Env.WebRootPath + "/imgs_resenasjuegos/" + rs.ResenaJuego.IdResenas + ".jpg", FileMode.Create);
                rs.Archivo.CopyTo(fs);
                fs.Close();
            }
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("Admin/Admin/Eliminar/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Eliminar(int id)
        {
            var r = Context.Resenas.FirstOrDefault(x => x.IdResenas == id);

            if (r == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            return View(r);
        }

        [HttpPost("Admin/Admin/Eliminar/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Eliminar(Resena r)
    {
        var resena = Context.Resenas.FirstOrDefault(x => x.IdResenas == r.IdResenas);

        if (resena == null)
        {
            ModelState.AddModelError("", "La raza no existe o ya ha sido eliminada.");
        }
        else
        {
            Context.Remove(resena);
            Context.SaveChanges();

            var path = Env.WebRootPath + "/imgs_resenasjuegos/" + resena.IdResenas + ".jpg";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return RedirectToAction("Index");
        }
        return View(r);
    }
    }

    
}
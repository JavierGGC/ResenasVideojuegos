using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ReseñasDeVideojuegos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReseñasDeVideojuegos
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(Options =>
                {
                    Options.LoginPath = "/Home/LoginUsuario";
                    Options.LogoutPath = "/Home/Logout";
                    Options.AccessDeniedPath = "/Home/AccesoDenegado";
                    Options.Cookie.Name = "galleta";
                }
                );

            services.AddDbContext<ReseñasDeVideojuegos.Models.resenajuegosContext>(
                x =>
          x.UseMySql("server=localhost;user=root;password=root;database=resenajuegos", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.27-mysql")));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseFileServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Admin}/{action=LoginAdmin}");
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}

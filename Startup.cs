using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBB.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreBB.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // 1. Se agrega el siguiente c�digo
            //  se cargan y activan los componentes de MVC, as� como los componentes de autenticaci�n
            services.AddMvc(option => option.EnableEndpointRouting = false); // Nota: se agrega como parametro option => option.EnableEndpointRouting = false, dado que en m�todo Configure daba error app.UseMvcWithDefaultRoute();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.LoginPath = "/User/LogIn"; // Ruta de logueo por defecto
                    option.AccessDeniedPath = "/Error/AccessDenied"; // Ruta para accesos no autorizados,  
                    option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                });

            // 13. Se agrega el siguiente c�digo, para aplicar la configuraci�n de la cadena de conexi�n
            services.AddDbContext<CoreBBContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 2. se agrega el siguiente c�digo
            app.UseExceptionHandler("/Error/Index"); // Ruta para manejar errores // se usa la acion del Index en ErrorController, para gestionar excepciones
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();            


            // El siguiente c�digo, ya se habia generado antes de agregar 2. (se 'elimina'.
            /*if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });*/
        }
    }
}

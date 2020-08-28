using AutoMapper;
using JMusik.Data;
using JMusik.WebApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace JMusik.WebApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        // Para tener acceso a las propiedades de configuracion de appsettings.json, debo agregar en el constructor de la clase IConfiguration e inicializar la variable publica del tipo correspondiente al a interfaz

        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            // Le decimos a serilog donde esta ubicado el archivo mediante la configuracion (appsettings.json) y que cree el log en ese archivo
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            Configuration = configuration;
        }

        // Agrego todo los servcios necesarios, inyeccion de depencencias, cadena de conexión, etc
        public void ConfigureServices(IServiceCollection services)
        {
            // Permitimos a la aplicación acceder a los controladores y actions
            // AddXmlDataContractSerializerFormatters nos permite comunicacion mediante XML
            // El Web Api tendra la disponibilidad de consulta de contenido en fomrato json, xml
            services.AddControllers(config =>
            {
                config.ReturnHttpNotAcceptable = true; // va a regresar un codigo de error 406, indicando que el tipo de formato indicado por el cliente no esta aceptado por la web api
            }).AddXmlDataContractSerializerFormatters();

            // Habilitando el automaper en el proyecto
            services.AddAutoMapper(typeof(Startup));

            // Cuando ya tenemos acceso a la interfaz de IConfiguration, añadimos la referencia del dbcontext para poder usar el db context en cualquier parte de la app
            services.AddDbContext<TiendaDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("TiendaDb")));

            ///// ------------Se usa la clase ServiceExtension donde estan todas las declaraciones de los servicios como buena practica---------- /////

            services.ConfigureDependencies();

            // Debido a que vamos a aceeder a la api desde diferentes aplicaciones, debemos habilitar cors mediante una politica
            services.ConfigureCors();

            services.ConfigureJWT(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Para la configuracion de las peticiones http de la aplicación
        // Canalización y procesamiento de péticiones http
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // Añadimos el serilog en la configuracion para las peticiones http
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                // Utilizamos una vista de excepción que nos muestra errores, en caso de estar en desarrollo
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Si no se encuentra en desarrollo, obligamos a utilizar el mecanismo de seguridad de HTTP, una politic de seguridad
                app.UseHsts();
            }

            app.UseRouting();

            // Habilitar cors y la autenticacion en el api
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            // En nuevas versioness se desacopla el funcionamiento de MVC, que obligaba a las peticiones a pasar por un controlador, el concepto basico de MVC
            // Ahora se trabajan con EndPoints, que son rutas de acceso a cualquier información
            app.UseEndpoints(endpoints =>
            {
                //Para usar controladores añadimos que los endopoints mapeen los controllers
                endpoints.MapControllers();

                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
            });
        }
    }
}
using AutoMapper;
using JMusik.Data;
using JMusik.Data.Contratos;
using JMusik.Data.Interfaces;
using JMusik.Data.Repositorios;
using JMusik.Models;
using JMusik.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Text;

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
            services.AddControllers();

            // Habilitando el automaper en el proyecto
            services.AddAutoMapper(typeof(Startup));

            // Cuando ya tenemos acceso a la interfaz de IConfiguration, añadimos la referencia del dbcontext para poder usar el db context en cualquier parte de la app
            services.AddDbContext<TiendaDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("TiendaDb"))
            );

            // Scoped significa que el servicio será creado y liberado por cada petición del cliente
            services.AddScoped<IProductoRepositorio, ProductoRepositorio>();

            services.AddScoped<IRepositorioGenerico<Perfil>, PerfilRepositorio>();

            services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();

            services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

            // Implementacion de usuarios con PasswordHasher, nos permite generar la encriptacion de los campos
            services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

            // Usamos singleton por que queremos solo una instancia para todo el ciclo de vida de nuestra aplicación
            services.AddSingleton<TokenService>();

            // Accedemos a la sección JwtSettings del archivo appsettings.json
            var jwtSettings = Configuration.GetSection("JwtSettings");

            // Obtenemos la clave secreta guardada en JwtSettings:SecretKey
            string secretKey = jwtSettings.GetValue<string>("SecretKey");

            // Obtenemos el tiempo de vida en minutos del Jwt guardada en JwtSettings:MinutesToExpiration
            int minutes = jwtSettings.GetValue<int>("MinutesToExpiration");

            // Obtenemos el valor del emisor del token en JwtSettings:Issuer
            string issuer = jwtSettings.GetValue<string>("Issuer");

            // Obtenemos el valor de la audiencia a la que está destinado el Jwt en JwtSettings:Audience
            string audience = jwtSettings.GetValue<string>("Audience");

            var key = Encoding.ASCII.GetBytes(secretKey);

            // Generando configuracion de jwt
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false; // debe ser true cuando lo publiquemos en producción, por que en prod debe ser https 
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(minutes)
                };
            });

            // Debido a que vamos a aceeder a la api desde diferentes aplicaciones, debemos habilitar cors mediante una politica
            services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                    );
                }
            );
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
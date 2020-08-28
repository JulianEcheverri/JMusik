using JMusik.Data.Contratos;
using JMusik.Data.Interfaces;
using JMusik.Data.Repositorios;
using JMusik.Models;
using JMusik.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace JMusik.WebApi.Extensions
{
    public static class ServiceExtensions
    {
        #region Implementación de Cors

        public static void ConfigureCors(this IServiceCollection services)
        {
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

        #endregion Implementación de Cors

        #region Implementación de JWT

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            // Accedemos a la sección JwtSettings del archivo appsettings.json
            var jwtSettings = configuration.GetSection("JwtSettings");

            // Obtenemos la clave secreta guardada en JwtSettings:SecretKey
            string secretKey = jwtSettings.GetValue<string>("SecretKey");

            // Obtenemos el tiempo de vida en minutos del Jwt guardada en JwtSettings:MinutesToExpiration
            int minutes = jwtSettings.GetValue<int>("MinutesToExpiration");

            // Obtenemos el valor del emisor del token en JwtSettings:Issuer
            string issuer = jwtSettings.GetValue<string>("Issuer");

            // Obtenemos el valor de la audiencia a la que está destinado el Jwt en JwtSettings:Audience
            string audience = jwtSettings.GetValue<string>("Audience");

            byte[] key = Encoding.ASCII.GetBytes(secretKey);

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
        }

        #endregion Implementación de JWT

        #region Implementación de las dependencias

        public static void ConfigureDependencies(this IServiceCollection services)
        {
            // Scoped significa que el servicio será creado y liberado por cada petición del cliente
            services.AddScoped<IProductoRepositorio, ProductoRepositorio>();

            services.AddScoped<IRepositorioGenerico<Perfil>, PerfilRepositorio>();

            services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();

            services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

            // Implementacion de usuarios con PasswordHasher, nos permite generar la encriptacion de los campos
            services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

            // Usamos singleton por que queremos solo una instancia para todo el ciclo de vida de nuestra aplicación
            services.AddSingleton<TokenService>();
        }

        #endregion Implementación de las dependencias
    }
}
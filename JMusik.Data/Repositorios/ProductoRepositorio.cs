using JMusik.Data.Interfaces;
using JMusik.Models;
using JMusik.Models.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMusik.Data.Repositorios
{
    // El repositorio es el en cargado de contener la logica de acceso a datos para que luego en el controlador se pueda tener acceso de una manera mas limpia
    // Se debe inyectar este servicio de acceso a datos en el startup.cs para poder usarlo en toda la aplicación
    public class ProductoRepositorio : IProductoRepositorio
    {
        private TiendaDbContext _contexto;
        private readonly ILogger<ProductoRepositorio> _logger;

        public ProductoRepositorio(TiendaDbContext contexto, ILogger<ProductoRepositorio> logger)
        {
            _contexto = contexto;
            _logger = logger;
        }

        public async Task<bool> Actualizar(Producto producto)
        {
            try
            {
                Producto productoEnDb = await ObtenerProductoAsync(producto.Id);
                productoEnDb.Nombre = producto.Nombre;
                productoEnDb.Precio = producto.Precio;

                //_contexto.Productos.Attach(producto);
                //_contexto.Entry(producto).State = EntityState.Modified;

                return await _contexto.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error em ${nameof(Actualizar)}: ${ ex.Message}");
                return false;
            }
        }

        public async Task<Producto> Agregar(Producto producto)
        {
            producto.Estatus = EstatusProducto.Activo;
            producto.FechaRegistro = DateTime.UtcNow;
            _contexto.Productos.Add(producto);
            try
            {
                await _contexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error em ${nameof(Agregar)}: ${ ex.Message}");
                return null;
            }

            return producto;
        }

        public async Task<bool> Eliminar(int id)
        {
            var producto = await _contexto.Productos
                                .SingleOrDefaultAsync(c => c.Id == id);

            producto.Estatus = EstatusProducto.Inactivo;
            _contexto.Productos.Attach(producto);
            _contexto.Entry(producto).State = EntityState.Modified;

            try
            {
                return (await _contexto.SaveChangesAsync() > 0);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error em ${nameof(Eliminar)}: ${ ex.Message}");
            }
            return false;
        }

        public async Task<Producto> ObtenerProductoAsync(int id)
        {
            return await _contexto.Productos
                        .SingleOrDefaultAsync(c => c.Id == id && c.Estatus == EstatusProducto.Activo);
        }

        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            return await _contexto.Productos
                .Where(u => u.Estatus == EstatusProducto.Activo)
                .OrderBy(u => u.Nombre)
                .ToListAsync();
        }
    }
}
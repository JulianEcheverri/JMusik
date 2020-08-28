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
    public class OrdenRepositorio : IOrdenRepositorio
    {
        private readonly TiendaDbContext _contexto;
        private readonly ILogger<OrdenRepositorio> _logger;
        private DbSet<Orden> _dbSet;

        public OrdenRepositorio(TiendaDbContext contexto, ILogger<OrdenRepositorio> logger)
        {
            _contexto = contexto;
            _logger = logger;
            _dbSet = _contexto.Set<Orden>();
        }

        public async Task<bool> Actualizar(Orden orden)
        {
            _dbSet.Attach(orden);
            _contexto.Entry(orden).State = EntityState.Modified;
            try
            {
                return await _contexto.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ${nameof(Actualizar)}: ${ex.Message}");
            }
            return false;
        }

        public async Task<Orden> Agregar(Orden orden)
        {
            orden.EstatusOrden = EstatusOrden.Activo;
            orden.FechaRegistro = DateTime.UtcNow;
            _dbSet.Add(orden);

            try
            {
                await _contexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ${nameof(Agregar)} ${ex.Message}");
            }
            return orden;
        }

        public async Task<bool> Eliminar(int id)
        {
            Orden orden = await _dbSet.SingleOrDefaultAsync(x => x.Id == id);
            orden.EstatusOrden = EstatusOrden.Inactivo;

            try
            {
                return await _contexto.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ${nameof(Eliminar)}: ${ex.Message}");
                return false;
            }
        }

        public async Task<Orden> ObtenerAsync(int id)
        {
            return await _dbSet.Include(x => x.Usuario).SingleOrDefaultAsync(x => x.Id == id && x.EstatusOrden == EstatusOrden.Activo);
        }

        public async Task<Orden> ObtenerConDetallesAsync(int id)
        {
            return await _dbSet.Include(x => x.Usuario)
                .Include(x => x.DetalleOrden).ThenInclude(x => x.Producto)
                .SingleOrDefaultAsync(x => x.Id == id && x.EstatusOrden == EstatusOrden.Activo);
        }

        public async Task<IEnumerable<Orden>> ObtenerTodosAsync()
        {
            return await _dbSet.Where(x => x.EstatusOrden == EstatusOrden.Activo)
                .Include(x => x.Usuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<Orden>> ObtenerTodosConDetallesAsync()
        {
            return await _dbSet.Where(x => x.EstatusOrden == EstatusOrden.Activo)
                .Include(x => x.Usuario)
                .Include(x => x.DetalleOrden).ThenInclude(x => x.Producto)
                .ToListAsync();
        }
    }
}
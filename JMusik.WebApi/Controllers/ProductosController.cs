using AutoMapper;
using JMusik.Data.Interfaces;
using JMusik.Dtos;
using JMusik.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JMusik.WebApi.Controllers
{
    [Authorize(Roles = "Administrador,Vendedor")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        // Se inyecta la dependencia mediante la interfaz
        private readonly IProductoRepositorio _productoRepositorio;

        // Inyectamos el mapper
        private readonly IMapper _mapper;

        // Inyectamos el logger para manejar los registros de error
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoRepositorio productoRepositorio, IMapper mapper, ILogger<ProductosController> logger)
        {
            _productoRepositorio = productoRepositorio;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Productos
        [HttpGet] // Atributo de enrutamiento que especifica y restringe que todas las llamadas get con esta estructura se enruten a este metodo
        [ProducesResponseType(StatusCodes.Status200OK)] // Filtro que especifica los status a devolver
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> Get()
        {
            try
            {
                // Se retorna el dto con los datos necesarios
                // Se obtiene la información mediante el mapeo
                List<Producto> productos = await _productoRepositorio.ObtenerProductosAsync();
                return _mapper.Map<List<ProductoDto>>(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ${nameof(Get)}: ${ ex.Message}");
                return BadRequest();
            }
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Filtro que especifica los status a devolver
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductoDto>> Get(int id)
        {
            Producto producto = await _productoRepositorio.ObtenerProductoAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return _mapper.Map<ProductoDto>(producto);
        }

        // POST: api/Productos
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // Filtro que especifica los status a devolver
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoDto>> Post(ProductoDto productoDto)
        {
            try
            {
                Producto producto = _mapper.Map<Producto>(productoDto);
                Producto nuevoProducto = await _productoRepositorio.Agregar(producto);
                if (nuevoProducto == null) return BadRequest();

                ProductoDto nuevoProductoDto = _mapper.Map<ProductoDto>(nuevoProducto);
                // Devolvemos el id del producto y el producto creado
                // CreatedAtAction crea un objeto de CreatedAtAction con el status 201
                return CreatedAtAction(nameof(Post), new { id = nuevoProductoDto.Id }, nuevoProductoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ${nameof(Post)}: ${ ex.Message}");
                return BadRequest();
            }
        }

        // PUT: api/Productos/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductoDto>> Put(int id, [FromBody] ProductoDto productoDto) // Indica que tenemos que pasar el producto desde el body en la solicitud
        {
            if (productoDto == null) return NotFound();
            if (productoDto.Id != id) return BadRequest();

            Producto producto = _mapper.Map<Producto>(productoDto);
            bool resultado = await _productoRepositorio.Actualizar(producto);
            if (!resultado) return BadRequest();

            return productoDto;
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool resultado = await _productoRepositorio.Eliminar(id);
                if (!resultado) return BadRequest();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ${nameof(Delete)} ${ ex.Message}");
                return BadRequest();
            }
        }
    }
}
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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesController : ControllerBase
    {
        private IOrdenRepositorio _ordenRepositorio;
        private readonly IMapper _mapper;

        public OrdenesController(IOrdenRepositorio ordenRepositorio, ILogger<OrdenesController> logger, IMapper mapper)
        {
            _ordenRepositorio = ordenRepositorio;
            _mapper = mapper;
        }

        // GET: api/ordenes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<OrdenDto>>> Get()
        {
            try
            {
                IEnumerable<Orden> ordenes = await _ordenRepositorio.ObtenerTodosAsync();
                return _mapper.Map<List<OrdenDto>>(ordenes);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // GET: api/ordenes/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrdenDto>> Get(int id)
        {
            var orden = await _ordenRepositorio.ObtenerAsync(id);
            if (orden == null) return NotFound();
            return _mapper.Map<OrdenDto>(orden);
        }

        // GET: api/ordenes/detalles
        [HttpGet]
        [Route("detalles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<OrdenDto>>> GetOrdenesConDetalle()
        {
            try
            {
                IEnumerable<Orden> ordenes = await _ordenRepositorio.ObtenerTodosConDetallesAsync();
                return _mapper.Map<List<OrdenDto>>(ordenes);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // GET: api/ordenes/5/detalles
        [HttpGet("{id}/detalles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrdenDto>> GetOrdenConDetalles(int id)
        {
            Orden orden = await _ordenRepositorio.ObtenerConDetallesAsync(id);
            if (orden == null) return NotFound();
            return _mapper.Map<OrdenDto>(orden);
        }

        // POST: api/ordenes
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrdenDto>> Post(OrdenDto ordenDto)
        {
            try
            {
                Orden orden = _mapper.Map<Orden>(ordenDto);

                Orden nuevaOrden = await _ordenRepositorio.Agregar(orden);
                if (nuevaOrden == null) return BadRequest();

                OrdenDto nuevaOrdenDto = _mapper.Map<OrdenDto>(nuevaOrden);
                return CreatedAtAction(nameof(Post), new { id = nuevaOrdenDto.Id }, nuevaOrdenDto);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // DELETE: api/ordenes/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool resultado = await _ordenRepositorio.Eliminar(id);
                if (!resultado) return BadRequest();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
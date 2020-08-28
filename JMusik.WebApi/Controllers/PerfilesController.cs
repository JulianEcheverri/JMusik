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
    [Authorize(Roles = "Administrador")]
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilesController : ControllerBase
    {
        private IRepositorioGenerico<Perfil> _perfilRepositorio;
        private readonly ILogger<PerfilesController> _logger;
        private readonly IMapper _mapper;

        public PerfilesController(IRepositorioGenerico<Perfil> perfilRepositorio, ILogger<PerfilesController> logger, IMapper mapper)
        {
            _perfilRepositorio = perfilRepositorio;
            _logger = logger;
            _mapper = mapper;
        }

        //// GET: api/perfiles
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PerfilDto>>> Get()
        {
            try
            {
                IEnumerable<Perfil> perfiles = await _perfilRepositorio.ObtenerTodosAsync();
                return _mapper.Map<List<PerfilDto>>(perfiles);
            }
            catch (Exception excepcion)
            {
                _logger.LogError($"Error en {nameof(Get)}: " + excepcion.Message);
                return BadRequest();
            }
        }

        // GET: api/perfiles/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PerfilDto>> Get(int id)
        {
            Perfil perfil = await _perfilRepositorio.ObtenerAsync(id);
            if (perfil == null)
            {
                return NotFound();
            }
            return _mapper.Map<PerfilDto>(perfil);
        }

        // POST: api/perfiles
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PerfilDto>> Post(PerfilDto perfilDto)
        {
            try
            {
                Perfil perfil = _mapper.Map<Perfil>(perfilDto);

                Perfil nuevoPerfil = await _perfilRepositorio.Agregar(perfil);
                if (nuevoPerfil == null)
                {
                    return BadRequest();
                }

                PerfilDto nuevoPerfilDto = _mapper.Map<PerfilDto>(nuevoPerfil);
                return CreatedAtAction(nameof(Post), new { id = nuevoPerfilDto.Id }, nuevoPerfilDto);
            }
            catch (Exception excepcion)
            {
                _logger.LogError($"Error en {nameof(Post)}: " + excepcion.Message);
                return BadRequest();
            }
        }

        // PUT: api/perfiles/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PerfilDto>> Put(int id, [FromBody] PerfilDto perfilDto)
        {
            if (perfilDto == null) return NotFound();

            Perfil perfil = _mapper.Map<Perfil>(perfilDto);
            bool resultado = await _perfilRepositorio.Actualizar(perfil);
            if (!resultado) return BadRequest();

            return perfilDto;
        }

        // DELETE: api/perfiles/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var resultado = await _perfilRepositorio.Eliminar(id);
                if (!resultado) return BadRequest();

                return NoContent();
            }
            catch (Exception excepcion)
            {
                _logger.LogError($"Error en {nameof(Delete)}: " + excepcion.Message);
                return BadRequest();
            }
        }
    }
}
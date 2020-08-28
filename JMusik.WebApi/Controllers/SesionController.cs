using AutoMapper;
using JMusik.Data.Contratos;
using JMusik.Dtos;
using JMusik.Models;
using JMusik.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JMusik.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SesionController : ControllerBase
    {
        private IUsuarioRepositorio _usuarioRepositorio;
        private IMapper _mapper;
        private TokenService _tokenService;

        public SesionController(IUsuarioRepositorio usuarioRepositorio, IMapper mapper, TokenService tokenService)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        //POST: api/sesion/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> PostLogin(LoginModelDto usuarioLogin)
        {
            Usuario datosLoginUsuario = _mapper.Map<Usuario>(usuarioLogin);
            var (resultado, usuario) = await _usuarioRepositorio.ValidarDatosLogin(datosLoginUsuario);

            if (!resultado) return BadRequest("Usuario/Contraseña Inválidos.");
            
            return _tokenService.GenerarToken(usuario);
        }
    }
}

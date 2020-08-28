using Microsoft.AspNetCore.Mvc; // Este namespace nos proporciona atributos que pueden ser usados para configurar el comportamiento de los controladores y metodos de acción de un web api
using System.Collections.Generic;

namespace JMusik.WebApi.Controllers
{
    // Para convertir en un controlador web api, debe heredar de la clase ControllerBase, la que nos proporciona clases y propiedades para el manejo de las peticiones HTTP

    // Si nuestra aplicación contase con paginas web y vistas, caracteristicas MVC, la clase debe heredar de Controller

    // Debemos agregar como propiedad a la clase la ruta e indicar que es un ApiController
    //[Route("api/values")] cambiamos el nombre "values" por un token de enrutamiento que lo que hace es tomar el nombre del controlador como valor de la ruta
    [Route("api/[controller]")]
    [ApiController] // nos indica que tenemos que realizar el enrutamiento de atributos destinado en route
    public class ValuesController : ControllerBase
    {
        //GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            // ApiController realiza las validaciones de respuesta de estatus automatico, pero se pueden indicar normalmente
            return new string[] { "Value1", "Value2" };
        }

        //GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        //POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        //PUT api/values
        [HttpPut]
        public void Put(int id, [FromBody] string value)
        {
        }

        //DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
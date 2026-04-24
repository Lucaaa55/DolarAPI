using Microsoft.AspNetCore.Mvc;
using Calcu.Models;
using Calcu.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calcu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculadoraController : ControllerBase
    {
        // GET: api/<OperationsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "/sumar", "/restar", "/multiplicar", "/dividir", "/sqrt", "/potencia" };
        }

        // GET api/<OperationsController>/5
        [HttpGet("sumar")]
        public IActionResult Sumar([FromBody] OperationDto dto)
        {
            float calc = dto.A + dto.B;

            Operation operation = new Operation();
            operation.OperationType = "Suma";
            operation.Result = calc;

            return Ok(operation);
        }

        [HttpGet("restar")]
        public IActionResult Restar([FromBody] OperationDto dto)
        {
            float calc = dto.A - dto.B;

            Operation operation = new Operation();
            operation.OperationType = "Resta";
            operation.Result = calc;

            return Ok(operation);
        }

        [HttpGet("sqrt")]
        public IActionResult Sqrt([FromBody] OperationDto dto)
        {
            float calc = (float)Math.Sqrt(dto.A);

            Operation operation = new Operation();
            operation.OperationType = "Raíz Cuadrada";
            operation.Result = calc;

            return Ok(operation);
        }
    }
}

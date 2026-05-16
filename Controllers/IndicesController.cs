using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dolarium.Interfaces;
using Dolarium.DTOs;
using Dolarium.Services;

namespace Dolarium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndicesController(
        IIndiceService indiceService
    ) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var indices = await indiceService.GetAllIndexes();
            return Ok(indices);
        }
    }
}

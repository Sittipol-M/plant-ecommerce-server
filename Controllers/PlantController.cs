using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace plant_ecommerce_server.Controllers
{
    [ApiController]
    [Route("plants")]
    public class PlantController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPlants()
        {
            return Ok();
        }
    }
}
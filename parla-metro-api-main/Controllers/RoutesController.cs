using Microsoft.AspNetCore.Mvc;
using parla_metro_api_main.Interfaces;
using parla_metro_api_main.Models.DTOs;

namespace parla_metro_api_main.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly IRoutesService _routeService;

        public RoutesController(IRoutesService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoutes()
        {
            var routes = await _routeService.GetAllRoutesAsync();
            return Ok(routes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRouteById(string id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound();
            return Ok(route);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] RouteDto routeDto)
        {
            var created = await _routeService.CreateRouteAsync(routeDto);
            return CreatedAtAction(nameof(GetRouteById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(string id, [FromBody] RouteDto routeDto)
        {
            var updated = await _routeService.UpdateRouteAsync(id, routeDto);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(string id)
        {
            var success = await _routeService.DeleteRouteAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}

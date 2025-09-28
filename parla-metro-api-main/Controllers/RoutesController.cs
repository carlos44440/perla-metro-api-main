using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using parla_metro_api_main.Models.Responses;
using parla_metro_api_main.Services.HttpClients;
using static parla_metro_api_main.Models.DTOs.RouteDto;

namespace parla_metro_api_main.Controllers
{
    /// <summary>
    /// Controlador para gestión de rutas del sistema Perla Metro.
    /// Maneja todas las operaciones CRUD relacionadas con rutas.
    /// </summary>
    [ApiController]
    [Route("gateway/routes")]
    // [Authorize]
    public class RoutesController : ControllerBase
    {
        private readonly IRoutesClient _routesClient;
        private readonly ILogger<RoutesController> _logger;

        public RoutesController(IRoutesClient routesClient, ILogger<RoutesController> logger)
        {
            _routesClient = routesClient;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las rutas disponibles del sistema
        /// </summary>
        /// <returns>Lista de rutas activas</returns>
        /// <response code="200">Lista de rutas obtenida exitosamente</response>
        /// <response code="401">Token de autorización requerido</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RouteDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<RouteDTO>>>> GetAllRoutes()
        {
            try
            {
                _logger.LogInformation("Solicitando todas las rutas del sistema");

                var routes = await _routesClient.GetAllRoutesAsync();

                // Creamos response
                var response = new ApiResponse<IEnumerable<RouteDTO>>
                {
                    Success = routes.Success,
                    Data = routes.Data,
                    Message = routes.Message,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener todas las rutas");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<IEnumerable<RouteDTO>>.ErrorResult("Error interno del servidor")
                );
            }
        }
    }
}

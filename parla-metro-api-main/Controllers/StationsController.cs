using Microsoft.AspNetCore.Mvc;
using parla_metro_api_main.Models.Requests;
using parla_metro_api_main.Services.HttpClients;
using parla_metro_api_main.Models.Responses;
using System.Text.Json;

namespace PerlaMetro.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StationsController : ControllerBase
    {
        private readonly IStationsClient _stationsClient;
        private readonly ILogger<StationsController> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public StationsController(IStationsClient stationsClient, ILogger<StationsController> logger)
        {
            _stationsClient = stationsClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Crear una nueva estación
        /// </summary>
        /// <param name="request">Datos de la nueva estación</param>
        /// <returns>Información de la estación creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(StationResponse), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        [ProducesResponseType(typeof(ErrorResponse), 502)]
        public async Task<ActionResult<StationResponse>> CreateStation([FromBody] CreateStationRequest request)
        {
            try
            {
                _logger.LogInformation("🚀 [GATEWAY] Iniciando creación de estación: {StationName}", request.Name);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(new ErrorResponse
                    {
                        Message = "Errores de validación en la petición",
                        Details = JsonSerializer.Serialize(errors),
                        StatusCode = 400,
                        Path = $"{Request.Method} {Request.Path}"
                    });
                }

                var serviceResponse = await _stationsClient.CreateStationAsync(request);
                var responseContent = await serviceResponse.Content.ReadAsStringAsync();

                if (serviceResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation("✅ [GATEWAY] Estación creada exitosamente: {StationName}", request.Name);
                    
                    var stationData = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                    var response = new StationResponse
                    {
                        Success = true,
                        Message = "Estación creada exitosamente",
                        Data = stationData
                    };

                    return CreatedAtAction(nameof(GetStationById), 
                        new { id = ExtractIdFromResponse(stationData) }, response);
                }

                _logger.LogWarning("⚠️ [GATEWAY] Error al crear estación - Status: {StatusCode}", serviceResponse.StatusCode);
                
                return StatusCode((int)serviceResponse.StatusCode, new ErrorResponse
                {
                    Message = "Error del servicio de estaciones",
                    Details = responseContent,
                    StatusCode = (int)serviceResponse.StatusCode,
                    Path = $"{Request.Method} {Request.Path}"
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "🔴 [GATEWAY] Error de comunicación al crear estación");
                throw; // Se maneja en ErrorHandlingMiddleware
            }
        }

        /// <summary>
        /// Obtener todas las estaciones con filtros opcionales
        /// </summary>
        /// <param name="nameFilter">Filtro por nombre (opcional)</param>
        /// <param name="isActive">Filtro por estado activo (opcional)</param>
        /// <param name="type">Filtro por tipo de estación (opcional)</param>
        /// <returns>Lista de estaciones</returns>
        [HttpGet]
        [ProducesResponseType(typeof(StationListResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 502)]
        public async Task<ActionResult<StationListResponse>> GetAllStations(
            [FromQuery] string? nameFilter = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int? type = null)
        {
            try
            {
                _logger.LogInformation("📋 [GATEWAY] Obteniendo lista de estaciones - Filtros: Name={NameFilter}, Active={IsActive}, Type={Type}", 
                    nameFilter, isActive, type);

                var serviceResponse = await _stationsClient.GetAllStationsAsync(nameFilter, isActive, type);
                var responseContent = await serviceResponse.Content.ReadAsStringAsync();

                if (serviceResponse.IsSuccessStatusCode)
                {
                    var stationsData = JsonSerializer.Deserialize<IEnumerable<object>>(responseContent, _jsonOptions);
                    var response = new StationListResponse
                    {
                        Success = true,
                        Message = "Estaciones obtenidas exitosamente",
                        Data = stationsData,
                        Count = stationsData?.Count() ?? 0
                    };

                    _logger.LogInformation("✅ [GATEWAY] Lista de estaciones obtenida - Count: {Count}", response.Count);
                    
                    return Ok(response);
                }

                _logger.LogWarning("⚠️ [GATEWAY] Error al obtener estaciones - Status: {StatusCode}", serviceResponse.StatusCode);
                
                return StatusCode((int)serviceResponse.StatusCode, new ErrorResponse
                {
                    Message = "Error del servicio de estaciones",
                    Details = responseContent,
                    StatusCode = (int)serviceResponse.StatusCode,
                    Path = $"{Request.Method} {Request.Path}"
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "🔴 [GATEWAY] Error de comunicación al obtener estaciones");
                throw; // Se maneja en ErrorHandlingMiddleware
            }
        }

        /// <summary>
        /// Obtener una estación específica por ID
        /// </summary>
        /// <summary>
        /// Obtener una estación específica por ID
        /// </summary>
        /// <param name="id">ID de la estación</param>
        /// <returns>Información detallada de la estación</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(StationResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 502)]
        public async Task<ActionResult<StationResponse>> GetStationById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "ID de estación inválido",
                        StatusCode = 400,
                        Path = $"{Request.Method} {Request.Path}"
                    });
                }

                _logger.LogInformation("🔍 [GATEWAY] Obteniendo estación por ID: {StationId}", id);

                var serviceResponse = await _stationsClient.GetStationByIdAsync(id);
                var responseContent = await serviceResponse.Content.ReadAsStringAsync();

                if (serviceResponse.IsSuccessStatusCode)
                {
                    var stationData = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                    var response = new StationResponse
                    {
                        Success = true,
                        Message = "Estación obtenida exitosamente",
                        Data = stationData
                    };

                    _logger.LogInformation("✅ [GATEWAY] Estación obtenida exitosamente: {StationId}", id);
                    return Ok(response);
                }

                if (serviceResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("⚠️ [GATEWAY] Estación no encontrada: {StationId}", id);
                    return NotFound(new ErrorResponse
                    {
                        Message = "Estación no encontrada",
                        StatusCode = 404,
                        Path = $"{Request.Method} {Request.Path}"
                    });
                }

                _logger.LogWarning("⚠️ [GATEWAY] Error al obtener estación {StationId} - Status: {StatusCode}", id, serviceResponse.StatusCode);

                return StatusCode((int)serviceResponse.StatusCode, new ErrorResponse
                {
                    Message = "Error del servicio de estaciones",
                    Details = responseContent,
                    StatusCode = (int)serviceResponse.StatusCode,
                    Path = $"{Request.Method} {Request.Path}"
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "🔴 [GATEWAY] Error de comunicación al obtener estación {StationId}", id);
                throw; // Se maneja en ErrorHandlingMiddleware
            }
        }

        /// <summary>
        /// Actualizar una estación existente
        /// </summary>
        /// <param name="id">ID de la estación</param>
        /// <param name="request">Datos a actualizar</param>
        /// <returns>Información actualizada de la estación</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(StationResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 409)]
        [ProducesResponseType(typeof(ErrorResponse), 502)]
        public async Task<ActionResult<StationResponse>> UpdateStation(Guid id, [FromBody] UpdateStationRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "ID de estación inválido",
                        StatusCode = 400,
                        Path = $"{Request.Method} {Request.Path}"
                    });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(new ErrorResponse
                    {
                        Message = "Errores de validación en la petición",
                        Details = JsonSerializer.Serialize(errors),
                        StatusCode = 400,
                        Path = $"{Request.Method} {Request.Path}"
                    });
                }

                _logger.LogInformation("🔄 [GATEWAY] Actualizando estación: {StationId}", id);

                var serviceResponse = await _stationsClient.UpdateStationAsync(id, request);
                var responseContent = await serviceResponse.Content.ReadAsStringAsync();

                if (serviceResponse.IsSuccessStatusCode)
                {
                    var stationData = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                    var response = new StationResponse
                    {
                        Success = true,
                        Message = "Estación actualizada exitosamente",
                        Data = stationData
                    };

                    _logger.LogInformation("✅ [GATEWAY] Estación actualizada exitosamente: {StationId}", id);
                    return Ok(response);
                }

                if (serviceResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("⚠️ [GATEWAY] Estación no encontrada para actualizar: {StationId}", id);
                    return NotFound(new ErrorResponse
                    {
                        Message = "Estación no encontrada",
                        StatusCode = 404,
                        Path = $"{Request.Method} {Request.Path}"
                    });
                }

                _logger.LogWarning("⚠️ [GATEWAY] Error al actualizar estación {StationId} - Status: {StatusCode}", id, serviceResponse.StatusCode);

                return StatusCode((int)serviceResponse.StatusCode, new ErrorResponse
                {
                    Message = "Error del servicio de estaciones",
                    Details = responseContent,
                    StatusCode = (int)serviceResponse.StatusCode,
                    Path = $"{Request.Method} {Request.Path}"
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "🔴 [GATEWAY] Error de comunicación al actualizar estación {StationId}", id);
                throw; // Se maneja en ErrorHandlingMiddleware
            }
        }

        /// <summary>
        /// Eliminar una estación (soft delete)
        /// </summary>
        /// <param name="id">ID de la estación</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(StationResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 502)]
        public async Task<ActionResult<StationResponse>> DeleteStation(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "ID de estación inválido",
                        StatusCode = 400,
                        Path = $"{Request.Method} {Request.Path}"
                    });
                }

                _logger.LogInformation("🗑️ [GATEWAY] Eliminando estación: {StationId}", id);

                var serviceResponse = await _stationsClient.DeleteStationAsync(id);
                var responseContent = await serviceResponse.Content.ReadAsStringAsync();

                if (serviceResponse.IsSuccessStatusCode)
                {
                    var responseData = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                    var response = new StationResponse
                    {
                        Success = true,
                        Message = "Estación eliminada exitosamente",
                        Data = responseData
                    };

                    _logger.LogInformation("✅ [GATEWAY] Estación eliminada exitosamente: {StationId}", id);
                    return Ok(response);
                }

                if (serviceResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("⚠️ [GATEWAY] Estación no encontrada para eliminar: {StationId}", id);
                    return NotFound(new ErrorResponse
                    {
                        Message = "Estación no encontrada",
                        StatusCode = 404,
                        Path = $"{Request.Method} {Request.Path}"
                    });
                }

                _logger.LogWarning("⚠️ [GATEWAY] Error al eliminar estación {StationId} - Status: {StatusCode}", id, serviceResponse.StatusCode);

                return StatusCode((int)serviceResponse.StatusCode, new ErrorResponse
                {
                    Message = "Error del servicio de estaciones",
                    Details = responseContent,
                    StatusCode = (int)serviceResponse.StatusCode,
                    Path = $"{Request.Method} {Request.Path}"
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "🔴 [GATEWAY] Error de comunicación al eliminar estación {StationId}", id);
                throw; // Se maneja en ErrorHandlingMiddleware
            }
        }

        /// <summary>
        /// Obtener estaciones eliminadas (para testing)
        /// </summary>
        /// <returns>Lista de estaciones eliminadas</returns>
        [HttpGet("deleted")]
        [ProducesResponseType(typeof(StationListResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 502)]
        public async Task<ActionResult<StationListResponse>> GetDeletedStations()
        {
            try
            {
                _logger.LogInformation("🔍 [GATEWAY] Obteniendo estaciones eliminadas");

                var serviceResponse = await _stationsClient.GetDeletedStationsAsync();
                var responseContent = await serviceResponse.Content.ReadAsStringAsync();

                if (serviceResponse.IsSuccessStatusCode)
                {
                    var stationsData = JsonSerializer.Deserialize<IEnumerable<object>>(responseContent, _jsonOptions);
                    var response = new StationListResponse
                    {
                        Success = true,
                        Message = "Estaciones eliminadas obtenidas exitosamente",
                        Data = stationsData,
                        Count = stationsData?.Count() ?? 0
                    };

                    _logger.LogInformation("✅ [GATEWAY] Estaciones eliminadas obtenidas - Count: {Count}", response.Count);
                    return Ok(response);
                }

                _logger.LogWarning("⚠️ [GATEWAY] Error al obtener estaciones eliminadas - Status: {StatusCode}", serviceResponse.StatusCode);

                return StatusCode((int)serviceResponse.StatusCode, new ErrorResponse
                {
                    Message = "Error del servicio de estaciones",
                    Details = responseContent,
                    StatusCode = (int)serviceResponse.StatusCode,
                    Path = $"{Request.Method} {Request.Path}"
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "🔴 [GATEWAY] Error de comunicación al obtener estaciones eliminadas");
                throw; // Se maneja en ErrorHandlingMiddleware
            }
        }

        /// <summary>
        /// Restaurar una estación eliminada
        /// </summary>
        /// <param name="id">ID de la estación a restaurar</param>
        /// <returns>Confirmación de restauración</returns>
        [HttpPost("{id:guid}/restore")]
        [ProducesResponseType(typeof(StationResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 502)]
        public async Task<ActionResult<StationResponse>> RestoreStation(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "ID de estación inválido",
                        StatusCode = 400,
                        Path = $"{Request.Method} {Request.Path}"
                    });
                }

                _logger.LogInformation("🔄 [GATEWAY] Restaurando estación: {StationId}", id);

                var serviceResponse = await _stationsClient.RestoreStationAsync(id);
                var responseContent = await serviceResponse.Content.ReadAsStringAsync();

                if (serviceResponse.IsSuccessStatusCode)
                {
                    var responseData = JsonSerializer.Deserialize<object>(responseContent, _jsonOptions);
                    var response = new StationResponse
                    {
                        Success = true,
                        Message = "Estación restaurada exitosamente",
                        Data = responseData
                    };

                    _logger.LogInformation("✅ [GATEWAY] Estación restaurada exitosamente: {StationId}", id);
                    return Ok(response);
                }

                if (serviceResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("⚠️ [GATEWAY] Estación no encontrada para restaurar: {StationId}", id);
                    return NotFound(new ErrorResponse
                    {
                        Message = "Estación no encontrada o no está eliminada",
                        StatusCode = 404,
                        Path = $"{Request.Method} {Request.Path}"
                    });
                }

                _logger.LogWarning("⚠️ [GATEWAY] Error al restaurar estación {StationId} - Status: {StatusCode}", id, serviceResponse.StatusCode);

                return StatusCode((int)serviceResponse.StatusCode, new ErrorResponse
                {
                    Message = "Error del servicio de estaciones",
                    Details = responseContent,
                    StatusCode = (int)serviceResponse.StatusCode,
                    Path = $"{Request.Method} {Request.Path}"
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "🔴 [GATEWAY] Error de comunicación al restaurar estación {StationId}", id);
                throw; // Se maneja en ErrorHandlingMiddleware
            }
        }

        /// <summary>
        /// Método auxiliar para extraer ID de la respuesta
        /// </summary>
        private static Guid? ExtractIdFromResponse(object? responseData)
        {
            if (responseData == null) return null;

            try
            {
                var jsonElement = (JsonElement)responseData;
                if (jsonElement.TryGetProperty("id", out var idProperty))
                {
                    return Guid.Parse(idProperty.GetString() ?? string.Empty);
                }
            }
            catch
            {
                // Si no se puede extraer el ID, devolver null
            }

            return null;
        }
    }
}
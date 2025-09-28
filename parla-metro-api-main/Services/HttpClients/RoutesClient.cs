using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using parla_metro_api_main.Models.Responses;
using static parla_metro_api_main.Models.DTOs.RouteDto;

namespace parla_metro_api_main.Services.HttpClients
{
    /// <summary>
    /// Cliente HTTP para comunicación con Routes Service
    /// Implementa todas las operaciones de gestión de rutas
    /// </summary>
    public class RoutesClient : IRoutesClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RoutesClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public RoutesClient(HttpClient httpClient, ILogger<RoutesClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Configuración de JSON para comunicación con el servicio
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
            };
        }

        /// <summary>
        /// Obtiene todas las rutas activas del sistema
        /// </summary>
        public async Task<ApiResponse<IEnumerable<RouteDTO>>> GetAllRoutesAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todas las rutas del Routes Service");

                var response = await _httpClient.GetAsync("/api/routes");

                if (response.IsSuccessStatusCode)
                {
                    var routes = await response.Content.ReadFromJsonAsync<IEnumerable<RouteDTO>>(
                        _jsonOptions
                    );
                    _logger.LogInformation(
                        "Se obtuvieron {Count} rutas exitosamente",
                        routes?.Count() ?? 0
                    );

                    return new ApiResponse<IEnumerable<RouteDTO>>
                    {
                        Success = true,
                        Data = routes ?? new List<RouteDTO>(),
                        Message = "Rutas obtenidas exitosamente",
                    };
                }

                _logger.LogWarning(
                    "Error al obtener rutas. Status: {StatusCode}",
                    response.StatusCode
                );
                return new ApiResponse<IEnumerable<RouteDTO>>
                {
                    Success = false,
                    Message = $"Error al obtener rutas: {response.StatusCode}",
                    ErrorData = new List<string> { response.ReasonPhrase ?? "Error desconocido" },
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de red al comunicarse con Routes Service");
                return new ApiResponse<IEnumerable<RouteDTO>>
                {
                    Success = false,
                    Message = "Error de comunicación con el servicio de rutas",
                    ErrorData = new List<string> { ex.Message },
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener rutas");
                return new ApiResponse<IEnumerable<RouteDTO>>
                {
                    Success = false,
                    Message = "Error interno al procesar la solicitud",
                    ErrorData = new List<string> { ex.Message },
                };
            }
        }
    }
}

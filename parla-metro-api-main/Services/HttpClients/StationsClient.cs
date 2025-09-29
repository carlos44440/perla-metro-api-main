using parla_metro_api_main.Models.Requests;
using parla_metro_api_main.Models.Responses;
using System.Text;
using System.Text.Json;

namespace parla_metro_api_main.Services.HttpClients
{
    public class StationsClient : IStationsClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StationsClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public StationsClient(HttpClient httpClient, ILogger<StationsClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<HttpResponseMessage> CreateStationAsync(CreateStationRequest request)
        {
            try
            {
                _logger.LogInformation("Enviando petición para crear estación: {StationName}", request.Name);
                
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/stations", content);
                
                _logger.LogInformation("Respuesta recibida del Station Service - Status: {StatusCode}", response.StatusCode);
                
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al crear estación: {Message}", ex.Message);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al crear estación: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> GetAllStationsAsync(string? nameFilter = null, bool? isActive = null, int? type = null)
        {
            try
            {
                var queryParams = new List<string>();
                
                if (!string.IsNullOrEmpty(nameFilter))
                    queryParams.Add($"nameFilter={Uri.EscapeDataString(nameFilter)}");
                if (isActive.HasValue)
                    queryParams.Add($"isActive={isActive.Value}");
                if (type.HasValue)
                    queryParams.Add($"type={type.Value}");

                var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var endpoint = $"/api/stations{queryString}";

                _logger.LogInformation("Obteniendo lista de estaciones con filtros: {Endpoint}", endpoint);

                var response = await _httpClient.GetAsync(endpoint);
                
                _logger.LogInformation("Respuesta recibida del Station Service - Status: {StatusCode}", response.StatusCode);
                
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al obtener estaciones: {Message}", ex.Message);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al obtener estaciones: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> GetStationByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Obteniendo estación por ID: {StationId}", id);

                var response = await _httpClient.GetAsync($"/api/stations/{id}");
                
                _logger.LogInformation("Respuesta recibida del Station Service - Status: {StatusCode}", response.StatusCode);
                
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al obtener estación {StationId}: {Message}", id, ex.Message);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al obtener estación {StationId}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> UpdateStationAsync(Guid id, UpdateStationRequest request)
        {
            try
            {
                _logger.LogInformation("Actualizando estación: {StationId}", id);

                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/stations/{id}", content);
                
                _logger.LogInformation("Respuesta recibida del Station Service - Status: {StatusCode}", response.StatusCode);
                
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al actualizar estación {StationId}: {Message}", id, ex.Message);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al actualizar estación {StationId}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> DeleteStationAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Eliminando estación: {StationId}", id);

                var response = await _httpClient.DeleteAsync($"/api/stations/{id}");
                
                _logger.LogInformation("Respuesta recibida del Station Service - Status: {StatusCode}", response.StatusCode);
                
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al eliminar estación {StationId}: {Message}", id, ex.Message);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al eliminar estación {StationId}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> GetDeletedStationsAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo estaciones eliminadas");

                var response = await _httpClient.GetAsync("/api/stations/deleted");
                
                _logger.LogInformation("Respuesta recibida del Station Service - Status: {StatusCode}", response.StatusCode);
                
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al obtener estaciones eliminadas: {Message}", ex.Message);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al obtener estaciones eliminadas: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> RestoreStationAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Restaurando estación: {StationId}", id);

                var response = await _httpClient.PostAsync($"/api/stations/{id}/restore", null);
                
                _logger.LogInformation("Respuesta recibida del Station Service - Status: {StatusCode}", response.StatusCode);
                
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión al restaurar estación {StationId}: {Message}", id, ex.Message);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al restaurar estación {StationId}: {Message}", id, ex.Message);
                throw;
            }
        }
    }
}
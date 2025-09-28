using parla_metro_api_main.Interfaces;
using parla_metro_api_main.Models.DTOs;

namespace parla_metro_api_main.Services
{
    public class RoutesService : IRoutesService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RoutesService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(
                _configuration["Services:Routes"]
                    ?? "https://perla-metro-routes-service-wf9c.onrender.com/api/"
            );
        }

        public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
        {
            var response = await _httpClient.GetAsync("routes");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<RouteDto>>()
                ?? new List<RouteDto>();
        }

        public async Task<RouteDto?> GetRouteByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"routes/{id}");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<RouteDto>();
        }

        public async Task<RouteDto> CreateRouteAsync(RouteDto routeDto)
        {
            var response = await _httpClient.PostAsJsonAsync("routes", routeDto);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<RouteDto>();
            if (result == null)
                throw new InvalidOperationException(
                    "Failed to deserialize RouteDto from response."
                );
            return result;
        }

        public async Task<RouteDto?> UpdateRouteAsync(string id, RouteDto routeDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"routes/{id}", routeDto);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<RouteDto>();
        }

        public async Task<bool> DeleteRouteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"routes/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}

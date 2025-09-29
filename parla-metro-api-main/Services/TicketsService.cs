using parla_metro_api_main.Helpers;
using parla_metro_api_main.Interfaces;
using parla_metro_api_main.Models.DTOs.Tickets;

namespace parla_metro_api_main.Services
{
    public class TicketsService : ITicketsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TicketsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(
                _configuration["Services:Tickets"]
                    ?? "https://tickets-api-deploy.onrender.com/api/"
            );
        }

        public async Task<TicketDto> CreateTicketAsync(CreateTicketDto newTicket)
        {
            var response = await _httpClient.PostAsJsonAsync("tickets", newTicket);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TicketDto>();
            if (result == null)
                throw new InvalidOperationException(
                    "Failed to deserialize TicketDto from response."
                );
            return result;
        }

        public async Task<GetTicketByIdDto?> GetTicketByIdAsync(Guid ticketId)
        {
            var response = await _httpClient.GetAsync($"tickets/{ticketId}");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<GetTicketByIdDto>();
        }

        public async Task<IEnumerable<GetAllTicketsDto>> GetAllTicketsAsync(QueryObjectTicket query)
        {
            var response = await _httpClient.GetAsync($"tickets?{query.ToQueryString()}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<GetAllTicketsDto>>()
                ?? new List<GetAllTicketsDto>();
        }

        public async Task<TicketDto?> UpdateTicketAsync(Guid ticketId, UpdateTicketDto updatedTicket)
        {
            var response = await _httpClient.PutAsJsonAsync($"tickets/{ticketId}", updatedTicket);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<TicketDto>();
        }

        public async Task<TicketDto?> DeleteTicketAsync(Guid ticketId)
        {
            var response = await _httpClient.DeleteAsync($"tickets/{ticketId}");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<TicketDto>();
        }
        
    }
}
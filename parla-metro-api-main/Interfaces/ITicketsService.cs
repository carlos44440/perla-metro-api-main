using parla_metro_api_main.Helpers;
using parla_metro_api_main.Models.DTOs.Tickets;

namespace parla_metro_api_main.Interfaces
{
    public interface ITicketsService
    {
        // Crear un ticket
        Task<TicketDto> CreateTicketAsync(CreateTicketDto newtTicket);

        // Obtener un ticket por su Id
        Task<GetTicketByIdDto?> GetTicketByIdAsync(Guid ticketId);

        // Listar todos los tickets
        Task<IEnumerable<GetAllTicketsDto>> GetAllTicketsAsync(QueryObjectTicket query);

        // Actualizar un ticket existente
        Task<TicketDto?> UpdateTicketAsync(Guid ticketId, UpdateTicketDto updatedTicket);

        // Eliminar un ticket por Id
        Task<TicketDto?> DeleteTicketAsync(Guid ticketId);
    }
}
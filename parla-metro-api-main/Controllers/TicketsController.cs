using Microsoft.AspNetCore.Mvc;
using parla_metro_api_main.Helpers;
using parla_metro_api_main.Interfaces;
using parla_metro_api_main.Models.DTOs;
using parla_metro_api_main.Models.DTOs.Tickets;

namespace parla_metro_api_main.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketsService _ticketService;

        public TicketsController(ITicketsService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        // [Authorize]
        public async Task<IActionResult> GetAllTickets([FromQuery] QueryObjectTicket query)
        {
            var tickets = await _ticketService.GetAllTicketsAsync(query);
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
                return NotFound();
            return Ok(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromForm] CreateTicketDto ticketDto)
        {
            var created = await _ticketService.CreateTicketAsync(ticketDto);
            return CreatedAtAction(nameof(GetTicketById), new { id = created.TicketID }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(Guid id, [FromForm] UpdateTicketDto ticketDto)
        {
            var updated = await _ticketService.UpdateTicketAsync(id, ticketDto);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        // [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            var deleted = await _ticketService.DeleteTicketAsync(id);
            if (deleted == null)
                return NotFound();
            return NoContent();
        }
    }
}
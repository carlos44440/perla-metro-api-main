using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace parla_metro_api_main.Models.DTOs.Tickets
{
    public class TicketDto
    {
        // Identificador único interno de MongoDB
        public string Id { get; set; } = null!;

        // Identificador único del ticket en formato GUID
        public Guid TicketID { get; set; } = Guid.NewGuid();
        
        // Identificador del pasajero
        public string IdPassenger { get; set; } = null!;

        // Fecha del ticket
        public DateTime Date { get; set;}

        // Tipo de ticket: "Ida" o "Vuelta"
        public string Type { get; set; } = null!;

        // Estado del ticket: "Activo", "Usado" o "Caducado"
        public string Status { get; set; } = null!;

        // Monto pagado por el ticket
        public decimal AmountPaid { get; set; }

        // Marca si el ticket ha sido eliminado lógicamente
        public bool IsDeleted { get; set; }
    }
}
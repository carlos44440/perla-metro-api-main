using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace parla_metro_api_main.Models.DTOs.Tickets
{
    public class GetTicketByIdDto
    {
        // Identificador único del ticket
        public Guid TicketID { get; set; }
        
        // Identificador del pasajero
        public string IdPassenger { get; set; }  = null!;
 
        // Fecha del ticket
        public DateTime Date { get; set;}

        // Tipo de ticket: "Ida" o "Vuelta"
        public string Type { get; set; }  = null!;

        // Monto pagado por el ticket
        public decimal AmountPaid { get; set; }   
    }
}
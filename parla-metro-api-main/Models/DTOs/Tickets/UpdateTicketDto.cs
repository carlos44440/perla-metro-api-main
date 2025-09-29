using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace parla_metro_api_main.Models.DTOs.Tickets
{
    public class UpdateTicketDto
    {
        // Fecha del ticket, por defecto se asigna la fecha actual
        public DateTime Date { get; set;} = DateTime.Now;

        // Tipo de ticket: "Ida" o "Vuelta"
        public string Type { get; set; } = null!;

        // Estado del ticket: "Activo", "Usado" o "Caducado"
        public string Status { get; set; } = null!;

        // Monto pagado por el ticket ( mayor que 0)
        public decimal AmountPaid { get; set; }
    }
}
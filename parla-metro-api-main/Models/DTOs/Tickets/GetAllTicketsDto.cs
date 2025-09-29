namespace parla_metro_api_main.Models.DTOs.Tickets
{
    public class GetAllTicketsDto
    {
        // Identificador único del ticket
        public Guid TicketID { get; set; }
        
        // Identificador del pasajero
        public string IdPassenger { get; set; }  = null!;
 
        // Fecha del ticket
        public DateTime Date { get; set;}

        // Tipo de ticket: "Ida" o "Vuelta"
        public string Type { get; set; }  = null!;
    
        // Estado del ticket: "Activo", "Usado" o "Caducado"
        public string Status { get; set; }  = null!;

        // Monto pagado por el ticket
        public decimal AmountPaid { get; set; }
    }
}
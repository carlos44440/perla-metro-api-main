namespace parla_metro_api_main.Models.DTOs.Tickets
{
    public class NewGetTicketByIdDto
    {
        // Identificador único del ticket
        public Guid TicketID { get; set; }
        
        // Nombre del pasajero
        public string NamePassenger { get; set; }  = null!;
 
        // Fecha del ticket
        public DateTime Date { get; set;}

        // Tipo de ticket: "Ida" o "Vuelta"
        public string Type { get; set; }  = null!;

        // Monto pagado por el ticket
        public decimal AmountPaid { get; set; }   
    }
}
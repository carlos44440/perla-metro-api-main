namespace parla_metro_api_main.Models.DTOs.Tickets
{
    public class CreateTicketDto
    {
        // Identificador del pasajero
        public string IdPassenger { get; set; }  = null!;
 
        // Fecha en que se genera el ticket
        public DateTime Date { get; set;} = DateTime.Now;

        // Tipo de ticket: puede ser solo "Ida" o "Vuelta"
        public string Type { get; set; }  = null!;

        // Estado del ticket: puede ser "Activo", "Usado" o "Caducado"
        // Por defecto se asigna "Activo"
        public string Status { get; set; }  = "Activo";

        // Monto pagado por el ticket 
        public decimal AmountPaid { get; set; }
    }
}
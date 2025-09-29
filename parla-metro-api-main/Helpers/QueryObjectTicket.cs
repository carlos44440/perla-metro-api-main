namespace parla_metro_api_main.Helpers
{
    public class QueryObjectTicket
    {
        // Texto de filtro general (puede aplicarse a ID de pasajero, fecha, etc.)
        public string? textFilter { get; set; } = string.Empty;

        // Filtrado por tipo de ticket: "Ida" o "Vuelta"
        public string? type { get; set; } = string.Empty;

        // Filtrado por estado del ticket: "Activo", "Usado" o "Caducado"
        public string? status { get; set; } = string.Empty;

        // Ordenamiento por monto pagado (campo)
        public string? sortByAmountPaid { get; set; } = string.Empty;

        // Ordenamiento por fecha del ticket (campo)
        public string? sortByDate { get; set; } = string.Empty;

        // Indica si el ordenamiento por monto debe ser descendente
        public bool isDescendingAmountPaid { get; set; } = false;

        // Indica si el ordenamiento por fecha debe ser descendente
        public bool isDescendingDate { get; set; } = false;
    }
}
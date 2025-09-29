using System.ComponentModel.DataAnnotations;

namespace parla_metro_api_main.Models.Requests
{
    public class CreateStationRequest
    {
        [Required(ErrorMessage = "El nombre de la estación es obligatorio")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de estación es obligatorio")]
        [Range(0, 2, ErrorMessage = "El tipo debe ser: 0=Origen, 1=Destino, 2=Intermedia")]
        public int Type { get; set; }
    }
}
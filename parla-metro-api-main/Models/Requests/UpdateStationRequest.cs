using System.ComponentModel.DataAnnotations;

namespace parla_metro_api_main.Models.Requests
{
    public class UpdateStationRequest
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        public string? Name { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "La ubicación debe tener entre 5 y 255 caracteres")]
        public string? Location { get; set; }

        [Range(0, 2, ErrorMessage = "El tipo debe ser: 0=Origen, 1=Destino, 2=Intermedia")]
        public int? Type { get; set; }

        public bool? IsActive { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace parla_metro_api_main.Models.DTOs
{
    public class RouteDto
    {
        /// <summary>
        /// DTO para transferencia de datos de rutas entre servicios
        /// </summary>
        public class RouteDTO
        {
            /// <summary>
            /// Identificador único de la ruta
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// Estación de origen
            /// </summary>
            public string Origin { get; set; } = string.Empty;

            /// <summary>
            /// Estación de destino
            /// </summary>
            public string Destination { get; set; } = string.Empty;

            /// <summary>
            /// Horario de inicio de la ruta
            /// </summary>
            public TimeSpan StartTime { get; set; }

            /// <summary>
            /// Horario de finalización de la ruta
            /// </summary>
            public TimeSpan EndTime { get; set; }

            /// <summary>
            /// Paradas intermedias de la ruta
            /// </summary>
            public List<string> Stops { get; set; } = new List<string>();

            /// <summary>
            /// Estado de la ruta (Active/Inactive)
            /// </summary>
            public string Status { get; set; } = string.Empty;
        }
    }
}

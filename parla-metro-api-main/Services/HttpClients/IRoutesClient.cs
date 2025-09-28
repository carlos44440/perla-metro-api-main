using parla_metro_api_main.Models.Responses;
using static parla_metro_api_main.Models.DTOs.RouteDto;

namespace parla_metro_api_main.Services.HttpClients
{
    /// <summary>
    /// Interface para comunicación con Routes Service.
    /// Maneja todas las operaciones relacionadas con rutas.
    /// </summary>
    public interface IRoutesClient
    {
        /// <summary>
        /// Obtiene todas las rutas activas del sistema
        /// </summary>
        /// <returns>Lista de rutas disponibles</returns>
        Task<ApiResponse<IEnumerable<RouteDTO>>> GetAllRoutesAsync();

        /// <summary>
        /// Obtiene una ruta específica por su ID
        /// </summary>
        /// <param name="routeId">Identificador único de la ruta</param>
        /// <returns>Información detallada de la ruta</returns>
        // Task<RouteDTO> GetRouteByIdAsync(Guid routeId);

        /// <summary>
        /// Crea una nueva ruta en el sistema
        /// </summary>
        /// <param name="request">Datos de la nueva ruta</param>
        /// <returns>Ruta creada con su ID asignado</returns>
        // Task<RouteDTO> CreateRouteAsync(CreateRouteRequest request);

        /// <summary>
        /// Actualiza los datos de una ruta existente
        /// </summary>
        /// <param name="routeId">ID de la ruta a actualizar</param>
        /// <param name="request">Nuevos datos de la ruta</param>
        /// <returns>Ruta actualizada</returns>
        // Task<RouteDTO> UpdateRouteAsync(Guid routeId, UpdateRouteRequest request);

        /// <summary>
        /// Elimina lógicamente una ruta (soft delete)
        /// </summary>
        /// <param name="routeId">ID de la ruta a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        // Task<bool> DeleteRouteAsync(Guid routeId);

        /// <summary>
        /// Valida si una ruta está disponible y activa
        /// Útil para validaciones antes de crear tickets
        /// </summary>
        /// <param name="routeId">ID de la ruta a validar</param>
        /// <returns>True si la ruta es válida y está activa</returns>
        // Task<bool> ValidateRouteAvailabilityAsync(Guid routeId);

        /// <summary>
        /// Obtiene rutas filtradas por origen y destino
        /// </summary>
        /// <param name="origin">Estación de origen</param>
        /// <param name="destination">Estación de destino</param>
        /// <returns>Lista de rutas que coinciden con el criterio</returns>
        // Task<IEnumerable<RouteDTO>> GetRoutesByOriginDestinationAsync(
        //     string origin,
        //     string destination
        // );

        /// <summary>
        /// Verifica el estado de salud del Routes Service
        /// </summary>
        /// <returns>Estado del servicio</returns>
        /// Task<bool> IsServiceHealthyAsync();
    }
}

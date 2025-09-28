using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using parla_metro_api_main.Models.DTOs;

namespace parla_metro_api_main.Interfaces
{
    public interface IRoutesService
    {
        Task<IEnumerable<RouteDto>> GetAllRoutesAsync();
        Task<RouteDto?> GetRouteByIdAsync(string id);
        Task<RouteDto> CreateRouteAsync(RouteDto routeDto);
        Task<RouteDto?> UpdateRouteAsync(string id, RouteDto routeDto);
        Task<bool> DeleteRouteAsync(string id);
    }
}

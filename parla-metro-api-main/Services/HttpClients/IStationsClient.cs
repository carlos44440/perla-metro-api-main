using parla_metro_api_main.Models.Requests;

namespace parla_metro_api_main.Services.HttpClients
{
    public interface IStationsClient
    {
        Task<HttpResponseMessage> CreateStationAsync(CreateStationRequest request);
        Task<HttpResponseMessage> GetAllStationsAsync(string? nameFilter = null, bool? isActive = null, int? type = null);
        Task<HttpResponseMessage> GetStationByIdAsync(Guid id);
        Task<HttpResponseMessage> UpdateStationAsync(Guid id, UpdateStationRequest request);
        Task<HttpResponseMessage> DeleteStationAsync(Guid id);
        Task<HttpResponseMessage> GetDeletedStationsAsync();
        Task<HttpResponseMessage> RestoreStationAsync(Guid id);
    }
}
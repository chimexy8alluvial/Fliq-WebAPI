using ConnectVibe.Application.Common.Models;

namespace ConnectVibe.Application.Common.Interfaces.Services.LocationServices
{
    public interface ILocationService
    {
        Task<LocationQueryResponse> GetAddressFromCoordinatesAsync(double latitude, double longitude);
    }
}
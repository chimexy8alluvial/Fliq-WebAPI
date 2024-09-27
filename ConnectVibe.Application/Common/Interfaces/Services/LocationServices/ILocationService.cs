using Fliq.Application.Common.Models;

namespace Fliq.Application.Common.Interfaces.Services.LocationServices
{
    public interface ILocationService
    {
        Task<LocationQueryResponse> GetAddressFromCoordinatesAsync(double latitude, double longitude);
    }
}
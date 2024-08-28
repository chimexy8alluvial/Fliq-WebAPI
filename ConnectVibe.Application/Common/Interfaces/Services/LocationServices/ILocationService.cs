using ConnectVibe.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Application.Common.Interfaces.Services.LocationServices
{
    public interface ILocationService
    {
        Task<LocationQueryResponse> GetAddressFromCoordinatesAsync(double latitude, double longitude);
    }
}
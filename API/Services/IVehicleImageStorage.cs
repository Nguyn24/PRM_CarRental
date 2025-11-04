using Microsoft.AspNetCore.Http;

namespace API.Services;

public interface IVehicleImageStorage
{
    Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);
}

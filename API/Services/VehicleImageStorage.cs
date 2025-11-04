using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Services;

public sealed class VehicleImageStorage : IVehicleImageStorage
{
    private const string VehicleImageFolder = "images/vehicles";
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<VehicleImageStorage> _logger;

    public VehicleImageStorage(IWebHostEnvironment environment, ILogger<VehicleImageStorage> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
        {
            throw new ArgumentException("Image file must not be empty.", nameof(file));
        }

        var webRoot = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRoot))
        {
            webRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");
        }

        var uploadFolder = Path.Combine(webRoot, VehicleImageFolder.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(uploadFolder);

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".png";
        }

        var safeName = string.Concat(Path.GetFileNameWithoutExtension(file.FileName)
            .Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

        if (string.IsNullOrWhiteSpace(safeName))
        {
            safeName = "vehicle";
        }

        var fileName = $"{safeName}_{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        _logger.LogInformation("Stored vehicle image at {Path}", filePath);

        var relativePath = "/" + Path
            .Combine(VehicleImageFolder, fileName)
            .Replace("\\", "/");

        return relativePath;
    }
}

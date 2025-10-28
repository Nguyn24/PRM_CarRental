using Application.Abstraction.Authentication;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Authentication;

public class Payload(IConfiguration configuration) : IPayload
{
    public string GoogleClientId => configuration["Google:ClientId"] ?? throw new InvalidOperationException("Google:ClientId is not configured");
}
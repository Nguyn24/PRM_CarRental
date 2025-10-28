using Infrastructure.Authentication;
using Microsoft.AspNetCore.Http;
using Application.Abstraction.Authentication;

namespace Infrastructure.Authentication;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public Guid UserId =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetUserId() ??
        throw new ApplicationException("Users context is unavailable");
}

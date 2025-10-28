using Domain.Common;
using Domain.Common;

namespace Domain.Users;

public class User : Entity
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public string? IDCardNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserStatus Status { get; set; }
    public bool IsVerified { get; set; }
    public string? AvatarUrl { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    

}
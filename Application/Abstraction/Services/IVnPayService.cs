namespace Application.Abstraction.Services;

public interface IVnPayService
{
    string CreatePaymentUrl(Guid userId, Guid membershipPlanId, decimal amount, string returnUrl, string? clientIp = null);
    VnPayResponse ProcessPaymentCallback(Dictionary<string, string> queryParams);
}

public class VnPayResponse
{
    public bool IsSuccess { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string OrderInfo { get; set; } = string.Empty;
    public string ResponseCode { get; set; } = string.Empty;
    public string TransactionNo { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string PayDate { get; set; } = string.Empty;
    public Dictionary<string, string> RawData { get; set; } = new();
}

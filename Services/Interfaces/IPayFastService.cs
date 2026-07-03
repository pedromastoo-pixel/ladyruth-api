using LadyRuth.API.DTOs.Orders;

namespace LadyRuth.API.Services.Interfaces;

public interface IPayFastService
{
    (string processUrl, Dictionary<string, string> fields) BuildPayment(OrderDto order);
    bool VerifySignature(IFormCollection form);
}

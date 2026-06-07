namespace LadyRuth.API.Helpers;

public static class OrderNumberGenerator
{
    /// <summary>Generates a unique order number, e.g. LR-20260001</summary>
    public static string Generate(int orderId)
        => $"LR-{DateTime.UtcNow:yyyy}{orderId:D4}";
}

namespace LadyRuth.API.Settings;

public class PayFastSettings
{
    public string MerchantId  { get; set; } = string.Empty;
    public string MerchantKey { get; set; } = string.Empty;
    public string Passphrase  { get; set; } = string.Empty;
    public bool   Sandbox     { get; set; } = true;
    public string ReturnUrlBase { get; set; } = string.Empty;
    public string CancelUrlBase { get; set; } = string.Empty;
    public string NotifyUrl     { get; set; } = string.Empty;

    public string ProcessUrl => Sandbox
        ? "https://sandbox.payfast.co.za/eng/process"
        : "https://www.payfast.co.za/eng/process";
}

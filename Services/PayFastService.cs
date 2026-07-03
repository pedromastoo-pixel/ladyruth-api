using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using LadyRuth.API.DTOs.Orders;
using LadyRuth.API.Services.Interfaces;
using LadyRuth.API.Settings;
using Microsoft.Extensions.Options;

namespace LadyRuth.API.Services;

public class PayFastService(IOptions<PayFastSettings> opts, ILogger<PayFastService> logger) : IPayFastService
{
    private readonly PayFastSettings _pf = opts.Value;

    public (string processUrl, Dictionary<string, string> fields) BuildPayment(OrderDto order)
    {
        var fields = new Dictionary<string, string>
        {
            ["merchant_id"]      = _pf.MerchantId,
            ["merchant_key"]     = _pf.MerchantKey,
            ["return_url"]       = $"{_pf.ReturnUrlBase}/order-confirmation/{order.OrderNumber}",
            ["cancel_url"]       = $"{_pf.CancelUrlBase}/checkout?cancelled=true",
            ["notify_url"]       = _pf.NotifyUrl,
            ["name_first"]       = order.GuestFirstName,
            ["name_last"]        = order.GuestLastName,
            ["email_address"]    = order.GuestEmail,
            ["m_payment_id"]     = order.OrderNumber,
            ["amount"]           = order.Total.ToString("F2", CultureInfo.InvariantCulture),
            ["item_name"]        = $"LadyRuth Order {order.OrderNumber}",
            ["item_description"] = $"{order.Items.Count} item(s)"
        };

        fields["signature"] = GenerateSignature(fields);
        return (_pf.ProcessUrl, fields);
    }

    public bool VerifySignature(IFormCollection form)
    {
        var received = form["signature"].ToString();
        var fields = form
            .Where(kv => kv.Key != "signature")
            .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());
        return string.Equals(received, GenerateSignature(fields), StringComparison.OrdinalIgnoreCase);
    }

    private string GenerateSignature(Dictionary<string, string> fields)
    {
        var pairs = fields
            .Where(kv => kv.Key != "signature" && !string.IsNullOrEmpty(kv.Value))
            .Select(kv => $"{kv.Key}={PhpUrlEncode(kv.Value)}");

        var str = string.Join("&", pairs);
        if (!string.IsNullOrEmpty(_pf.Passphrase))
            str += $"&passphrase={PhpUrlEncode(_pf.Passphrase)}";

        logger.LogInformation("PayFast signature string: {Str}", str);

        return Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(str))).ToLower();
    }

    // Matches PHP urlencode(): only A-Z a-z 0-9 - _ . are left unencoded; space → +
    private static string PhpUrlEncode(string value)
    {
        var sb = new StringBuilder();
        foreach (var b in Encoding.UTF8.GetBytes(value))
        {
            char c = (char)b;
            if (c == ' ')
                sb.Append('+');
            else if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') ||
                     (c >= '0' && c <= '9') || c == '-' || c == '_' || c == '.')
                sb.Append(c);
            else
                sb.Append($"%{b:X2}");
        }
        return sb.ToString();
    }
}

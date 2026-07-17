using LadyRuth.API.DTOs.Orders;
using LadyRuth.API.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LadyRuth.API.Services;

public class EmailService(IConfiguration config, ILogger<EmailService> logger) : IEmailService
{
    public async Task SendOrderConfirmationAsync(OrderDto order)
    {
        var fromEmail = config["Smtp:FromEmail"] ?? "orders@ladyruth.co.za";
        var fromName  = config["Smtp:FromName"]  ?? "LadyRuth";

        var itemsHtml = string.Join("", order.Items.Select(i => $"""
            <tr>
              <td style="padding:8px 0;border-bottom:1px solid #f0f0f0">{i.ProductName} – {i.Colour}, {i.Size}</td>
              <td style="padding:8px 0;border-bottom:1px solid #f0f0f0;text-align:center">{i.Quantity}</td>
              <td style="padding:8px 0;border-bottom:1px solid #f0f0f0;text-align:right">R {i.UnitPrice:F2}</td>
              <td style="padding:8px 0;border-bottom:1px solid #f0f0f0;text-align:right">R {i.LineTotal:F2}</td>
            </tr>
            """));

        var html = $"""
            <!DOCTYPE html>
            <html>
            <body style="font-family:Arial,sans-serif;color:#333;max-width:600px;margin:0 auto;padding:20px">
              <div style="text-align:center;margin-bottom:32px">
                <h1 style="color:#c8a96e;margin:0">LadyRuth</h1>
                <p style="color:#888;margin:4px 0">Fashion &amp; Style</p>
              </div>

              <h2 style="font-size:20px">Order Confirmed! 🎉</h2>
              <p>Hi {order.GuestFirstName},</p>
              <p>Thank you for your order. We've received your payment and will start preparing it right away.</p>

              <div style="background:#f9f9f9;border-radius:8px;padding:16px;margin:24px 0">
                <p style="margin:0 0 4px"><strong>Order number:</strong> {order.OrderNumber}</p>
                <p style="margin:0"><strong>Date:</strong> {order.CreatedAt:dd MMMM yyyy}</p>
              </div>

              <h3 style="font-size:16px;border-bottom:2px solid #c8a96e;padding-bottom:8px">Order Summary</h3>
              <table style="width:100%;border-collapse:collapse;font-size:14px">
                <thead>
                  <tr style="color:#888;font-size:12px">
                    <th style="text-align:left;padding-bottom:8px">Item</th>
                    <th style="text-align:center;padding-bottom:8px">Qty</th>
                    <th style="text-align:right;padding-bottom:8px">Price</th>
                    <th style="text-align:right;padding-bottom:8px">Total</th>
                  </tr>
                </thead>
                <tbody>{itemsHtml}</tbody>
              </table>

              <table style="width:100%;font-size:14px;margin-top:16px">
                <tr><td>Subtotal</td><td style="text-align:right">R {order.SubTotal:F2}</td></tr>
                <tr><td>Shipping</td><td style="text-align:right">R {order.ShippingFee:F2}</td></tr>
                <tr style="font-weight:bold;font-size:16px">
                  <td style="padding-top:8px">Total</td>
                  <td style="text-align:right;padding-top:8px">R {order.Total:F2}</td>
                </tr>
              </table>

              <h3 style="font-size:16px;border-bottom:2px solid #c8a96e;padding-bottom:8px;margin-top:32px">Delivery Address</h3>
              <p style="margin:0">{order.GuestFirstName} {order.GuestLastName}</p>
              <p style="margin:0">{order.AddressLine1}</p>
              {(string.IsNullOrWhiteSpace(order.AddressLine2) ? "" : $"<p style=\"margin:0\">{order.AddressLine2}</p>")}
              <p style="margin:0">{order.City}, {order.Province} {order.PostalCode}</p>

              <p style="margin-top:32px;color:#888;font-size:13px">
                If you have any questions, reply to this email or contact us at support@ladyruth.co.za.
              </p>
              <p style="color:#c8a96e;font-weight:bold">— The LadyRuth Team</p>
            </body>
            </html>
            """;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(MailboxAddress.Parse(order.GuestEmail));
        message.Subject = $"Order Confirmed – {order.OrderNumber}";
        message.Body = new TextPart("html") { Text = html };

        try
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                config["Smtp:Host"],
                config.GetValue<int>("Smtp:Port", 587),
                SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(config["Smtp:Username"], config["Smtp:Password"]);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

            logger.LogInformation("Order confirmation email sent to {Email} for order {OrderNumber}",
                order.GuestEmail, order.OrderNumber);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send order confirmation email for order {OrderNumber}", order.OrderNumber);
        }
    }
}

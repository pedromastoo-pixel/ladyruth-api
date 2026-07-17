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

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(MailboxAddress.Parse(order.GuestEmail));
        message.Subject = $"Order Confirmed – {order.OrderNumber}";
        message.Body = new TextPart("html") { Text = EmailTemplates.OrderConfirmation(order) };

        try
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                config["Smtp:Host"],
                config.GetValue<int>("Smtp:Port", 465),
                SecureSocketOptions.SslOnConnect);
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

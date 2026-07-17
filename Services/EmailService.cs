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

        await Send(message);
    }

    public async Task SendStatusUpdateAsync(OrderDto order)
    {
        var fromEmail = config["Smtp:FromEmail"] ?? "orders@ladyruth.co.za";
        var fromName  = config["Smtp:FromName"]  ?? "LadyRuth";

        var subject = order.Status switch
        {
            "Shipped"   => $"Your order {order.OrderNumber} has shipped 🚚",
            "Delivered" => $"Your order {order.OrderNumber} has been delivered ✅",
            "Cancelled" => $"Your order {order.OrderNumber} has been cancelled",
            _           => $"Update on your order {order.OrderNumber}"
        };

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(MailboxAddress.Parse(order.GuestEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = EmailTemplates.StatusUpdate(order) };

        await Send(message);
        logger.LogInformation("Status update email ({Status}) sent to {Email} for order {OrderNumber}",
            order.Status, order.GuestEmail, order.OrderNumber);
    }

    private async Task Send(MimeMessage message)
    {
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

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {To}", message.To);
        }
    }
}

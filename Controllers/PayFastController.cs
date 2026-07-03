using LadyRuth.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LadyRuth.API.Controllers;

[ApiController]
[Route("api/payfast")]
public class PayFastController(
    IOrderService orderService,
    IPayFastService payFastService,
    ILogger<PayFastController> logger) : ControllerBase
{
    // PayFast posts ITN (Instant Transaction Notification) here after payment
    [HttpPost("notify")]
    public async Task<IActionResult> Notify()
    {
        var form = await Request.ReadFormAsync();

        if (!payFastService.VerifySignature(form))
        {
            logger.LogWarning("PayFast ITN: signature verification failed. IP={Ip}", HttpContext.Connection.RemoteIpAddress);
            return BadRequest();
        }

        var paymentStatus = form["payment_status"].ToString();
        var orderNumber   = form["m_payment_id"].ToString();
        var pfPaymentId   = form["pf_payment_id"].ToString();

        logger.LogInformation("PayFast ITN: order={Order} status={Status} pf_id={PfId}",
            orderNumber, paymentStatus, pfPaymentId);

        await orderService.UpdatePaymentStatusAsync(orderNumber, pfPaymentId, paymentStatus == "COMPLETE");

        return Ok();
    }
}

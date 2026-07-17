using LadyRuth.API.DTOs.Orders;

namespace LadyRuth.API.Services;

public static class EmailTemplates
{
    public static string StatusUpdate(OrderDto order)
    {
        var (icon, heading, body) = order.Status switch
        {
            "Shipped"   => ("🚚", "Your order is on its way!", $"Great news, <strong>{order.GuestFirstName}</strong>! Your order has been shipped and is heading to you."),
            "Delivered" => ("✅", "Your order has been delivered!", $"Hi <strong>{order.GuestFirstName}</strong>, your order has been delivered. We hope you love your purchase!"),
            "Cancelled" => ("❌", "Your order has been cancelled", $"Hi <strong>{order.GuestFirstName}</strong>, your order has been cancelled. If you have questions, please contact us."),
            _           => ("⚙️", "Your order is being processed", $"Hi <strong>{order.GuestFirstName}</strong>, we're preparing your order and will update you when it ships.")
        };

        var trackUrl = $"https://ladyruth.co.za/track-order/{order.OrderNumber}";
        var notesRow = string.IsNullOrWhiteSpace(order.AdminNotes) ? "" : $"""
            <tr>
              <td style="background:#fffbea;padding:16px 40px;border-left:1px solid #f5e6f0;border-right:1px solid #f5e6f0;">
                <p style="margin:0 0 4px;font-size:11px;font-weight:700;letter-spacing:0.1em;text-transform:uppercase;color:#f59f00;">Note from us</p>
                <p style="margin:0;font-size:14px;color:#555;">{order.AdminNotes}</p>
              </td>
            </tr>
            """;

        return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="UTF-8" />
              <meta name="viewport" content="width=device-width,initial-scale=1" />
              <title>{heading} – {order.OrderNumber}</title>
            </head>
            <body style="margin:0;padding:0;background-color:#fdf2f8;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;">
              <table width="100%" cellpadding="0" cellspacing="0" border="0" style="background-color:#fdf2f8;padding:40px 16px;">
                <tr><td align="center">
                  <table width="600" cellpadding="0" cellspacing="0" border="0" style="max-width:600px;width:100%;">

                    <tr>
                      <td style="background:linear-gradient(135deg,#e8468c 0%,#c4307a 100%);border-radius:16px 16px 0 0;padding:40px 40px 32px;text-align:center;">
                        <p style="margin:0 0 6px;font-size:11px;font-weight:700;letter-spacing:0.18em;text-transform:uppercase;color:rgba(255,255,255,0.75)">Fashion &amp; Style</p>
                        <h1 style="margin:0;font-size:38px;font-weight:800;color:#ffffff;letter-spacing:-0.5px;">LadyRuth</h1>
                        <div style="margin:20px auto 0;width:48px;height:2px;background:rgba(255,255,255,0.4);border-radius:2px;"></div>
                      </td>
                    </tr>

                    <tr>
                      <td style="background:#ffffff;padding:36px 40px 28px;text-align:center;border-left:1px solid #f5e6f0;border-right:1px solid #f5e6f0;">
                        <div style="display:inline-block;background:#fff0f9;border-radius:50%;width:72px;height:72px;line-height:72px;font-size:32px;margin-bottom:20px;">{icon}</div>
                        <h2 style="margin:0 0 10px;font-size:24px;font-weight:700;color:#2d2d2d;">{heading}</h2>
                        <p style="margin:0;font-size:15px;color:#777;line-height:1.6;">{body}</p>
                      </td>
                    </tr>

                    <tr>
                      <td style="background:#fff8fc;padding:0 40px;border-left:1px solid #f5e6f0;border-right:1px solid #f5e6f0;">
                        <table width="100%" cellpadding="0" cellspacing="0" border="0" style="border-top:2px solid #f5e6f0;border-bottom:2px solid #f5e6f0;padding:20px 0;">
                          <tr>
                            <td style="width:50%;padding:6px 0;">
                              <p style="margin:0;font-size:11px;font-weight:700;letter-spacing:0.1em;text-transform:uppercase;color:#c4307a;">Order Number</p>
                              <p style="margin:4px 0 0;font-size:18px;font-weight:800;color:#2d2d2d;">{order.OrderNumber}</p>
                            </td>
                            <td style="width:50%;padding:6px 0;text-align:right;">
                              <p style="margin:0;font-size:11px;font-weight:700;letter-spacing:0.1em;text-transform:uppercase;color:#c4307a;">Status</p>
                              <p style="margin:4px 0 0;font-size:15px;font-weight:700;color:#2d2d2d;">{order.Status}</p>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>

                    {notesRow}

                    <tr>
                      <td style="background:#ffffff;padding:28px 40px;text-align:center;border-left:1px solid #f5e6f0;border-right:1px solid #f5e6f0;">
                        <a href="{trackUrl}" style="display:inline-block;background:linear-gradient(135deg,#e8468c,#c4307a);color:#ffffff;font-size:15px;font-weight:700;text-decoration:none;padding:14px 36px;border-radius:50px;letter-spacing:0.02em;">
                          Track My Order
                        </a>
                      </td>
                    </tr>

                    <tr>
                      <td style="background:linear-gradient(135deg,#2d2d2d 0%,#1a1a1a 100%);border-radius:0 0 16px 16px;padding:32px 40px;text-align:center;">
                        <p style="margin:0 0 8px;font-size:18px;font-weight:800;color:#e8468c;letter-spacing:-0.3px;">LadyRuth</p>
                        <p style="margin:0 0 16px;font-size:12px;color:#888;">Fashion &amp; Style | South Africa</p>
                        <div style="margin:0 0 16px;height:1px;background:#333;"></div>
                        <p style="margin:0 0 6px;font-size:12px;color:#888;">Questions? <a href="mailto:support@ladyruth.co.za" style="color:#e8468c;text-decoration:none;">support@ladyruth.co.za</a></p>
                        <p style="margin:0;font-size:11px;color:#555;">&copy; {DateTime.UtcNow.Year} LadyRuth. All rights reserved.</p>
                      </td>
                    </tr>

                  </table>
                </td></tr>
              </table>
            </body>
            </html>
            """;
    }

    public static string OrderConfirmation(OrderDto order)
    {
        var itemRows = string.Join("\n", order.Items.Select(i => $"""
            <tr>
              <td style="padding:14px 16px;border-bottom:1px solid #f5e6f0;vertical-align:top">
                <span style="font-weight:600;color:#2d2d2d;font-size:14px">{i.ProductName}</span><br>
                <span style="color:#999;font-size:12px">{i.Colour} &bull; {i.Size}</span>
              </td>
              <td style="padding:14px 16px;border-bottom:1px solid #f5e6f0;text-align:center;vertical-align:top;color:#555;font-size:14px">{i.Quantity}</td>
              <td style="padding:14px 16px;border-bottom:1px solid #f5e6f0;text-align:right;vertical-align:top;color:#555;font-size:14px">R&nbsp;{i.UnitPrice:F2}</td>
              <td style="padding:14px 16px;border-bottom:1px solid #f5e6f0;text-align:right;vertical-align:top;font-weight:600;color:#2d2d2d;font-size:14px">R&nbsp;{i.LineTotal:F2}</td>
            </tr>
            """));

        var addressLine2 = string.IsNullOrWhiteSpace(order.AddressLine2)
            ? ""
            : $"<tr><td style=\"padding:2px 0;color:#555;font-size:14px\">{order.AddressLine2}</td></tr>";

        return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="UTF-8" />
              <meta name="viewport" content="width=device-width,initial-scale=1" />
              <title>Order Confirmed – {order.OrderNumber}</title>
            </head>
            <body style="margin:0;padding:0;background-color:#fdf2f8;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;">

              <!-- Wrapper -->
              <table width="100%" cellpadding="0" cellspacing="0" border="0" style="background-color:#fdf2f8;padding:40px 16px;">
                <tr>
                  <td align="center">
                    <table width="600" cellpadding="0" cellspacing="0" border="0" style="max-width:600px;width:100%;">

                      <!-- ── HEADER ─────────────────────────────────────────── -->
                      <tr>
                        <td style="background:linear-gradient(135deg,#e8468c 0%,#c4307a 100%);border-radius:16px 16px 0 0;padding:40px 40px 32px;text-align:center;">
                          <p style="margin:0 0 6px;font-size:11px;font-weight:700;letter-spacing:0.18em;text-transform:uppercase;color:rgba(255,255,255,0.75)">Fashion &amp; Style</p>
                          <h1 style="margin:0;font-size:38px;font-weight:800;color:#ffffff;letter-spacing:-0.5px;">LadyRuth</h1>
                          <div style="margin:20px auto 0;width:48px;height:2px;background:rgba(255,255,255,0.4);border-radius:2px;"></div>
                        </td>
                      </tr>

                      <!-- ── HERO BANNER ─────────────────────────────────────── -->
                      <tr>
                        <td style="background:#ffffff;padding:36px 40px 28px;text-align:center;border-left:1px solid #f5e6f0;border-right:1px solid #f5e6f0;">
                          <div style="display:inline-block;background:#fff0f9;border-radius:50%;width:72px;height:72px;line-height:72px;font-size:32px;margin-bottom:20px;">🎀</div>
                          <h2 style="margin:0 0 10px;font-size:26px;font-weight:700;color:#2d2d2d;">Your order is confirmed!</h2>
                          <p style="margin:0;font-size:15px;color:#777;line-height:1.6;">Hi <strong style="color:#2d2d2d">{order.GuestFirstName}</strong>, thank you for shopping with us.<br>We've received your payment and are getting your order ready.</p>
                        </td>
                      </tr>

                      <!-- ── ORDER META ──────────────────────────────────────── -->
                      <tr>
                        <td style="background:#fff8fc;padding:0 40px;border-left:1px solid #f5e6f0;border-right:1px solid #f5e6f0;">
                          <table width="100%" cellpadding="0" cellspacing="0" border="0" style="border-top:2px solid #f5e6f0;border-bottom:2px solid #f5e6f0;padding:20px 0;">
                            <tr>
                              <td style="width:50%;padding:6px 0;">
                                <p style="margin:0;font-size:11px;font-weight:700;letter-spacing:0.1em;text-transform:uppercase;color:#c4307a;">Order Number</p>
                                <p style="margin:4px 0 0;font-size:18px;font-weight:800;color:#2d2d2d;">{order.OrderNumber}</p>
                              </td>
                              <td style="width:50%;padding:6px 0;text-align:right;">
                                <p style="margin:0;font-size:11px;font-weight:700;letter-spacing:0.1em;text-transform:uppercase;color:#c4307a;">Order Date</p>
                                <p style="margin:4px 0 0;font-size:15px;font-weight:600;color:#2d2d2d;">{order.CreatedAt:dd MMMM yyyy}</p>
                              </td>
                            </tr>
                          </table>
                        </td>
                      </tr>

                      <!-- ── ORDER ITEMS ─────────────────────────────────────── -->
                      <tr>
                        <td style="background:#ffffff;padding:28px 40px 0;border-left:1px solid #f5e6f0;border-right:1px solid #f5e6f0;">
                          <p style="margin:0 0 14px;font-size:12px;font-weight:700;letter-spacing:0.12em;text-transform:uppercase;color:#c4307a;">Items Ordered</p>
                          <table width="100%" cellpadding="0" cellspacing="0" border="0" style="border-collapse:collapse;">
                            <thead>
                              <tr style="background:#fdf2f8;">
                                <th style="padding:10px 16px;text-align:left;font-size:11px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#999;border-radius:8px 0 0 8px;">Product</th>
                                <th style="padding:10px 16px;text-align:center;font-size:11px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#999;">Qty</th>
                                <th style="padding:10px 16px;text-align:right;font-size:11px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#999;">Price</th>
                                <th style="padding:10px 16px;text-align:right;font-size:11px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#999;border-radius:0 8px 8px 0;">Total</th>
                              </tr>
                            </thead>
                            <tbody>
                              {itemRows}
                            </tbody>
                          </table>
                        </td>
                      </tr>

                      <!-- ── TOTALS ──────────────────────────────────────────── -->
                      <tr>
                        <td style="background:#ffffff;padding:0 40px 28px;border-left:1px solid #f5e6f0;border-right:1px solid #f5e6f0;">
                          <table width="100%" cellpadding="0" cellspacing="0" border="0" style="margin-top:8px;">
                            <tr>
                              <td style="padding:6px 16px 6px 0;font-size:14px;color:#777;">Subtotal</td>
                              <td style="padding:6px 0;font-size:14px;color:#555;text-align:right;">R&nbsp;{order.SubTotal:F2}</td>
                            </tr>
                            <tr>
                              <td style="padding:6px 16px 6px 0;font-size:14px;color:#777;">Shipping</td>
                              <td style="padding:6px 0;font-size:14px;color:#555;text-align:right;">R&nbsp;{order.ShippingFee:F2}</td>
                            </tr>
                            <tr>
                              <td colspan="2" style="padding:4px 0;"><div style="height:1px;background:#f5e6f0;"></div></td>
                            </tr>
                            <tr>
                              <td style="padding:12px 16px 12px 0;font-size:17px;font-weight:800;color:#2d2d2d;">Total Paid</td>
                              <td style="padding:12px 0;font-size:20px;font-weight:800;color:#e8468c;text-align:right;">R&nbsp;{order.Total:F2}</td>
                            </tr>
                          </table>
                        </td>
                      </tr>

                      <!-- ── DELIVERY ADDRESS ────────────────────────────────── -->
                      <tr>
                        <td style="background:#fff8fc;padding:24px 40px;border-left:1px solid #f5e6f0;border-right:1px solid #f5e6f0;">
                          <p style="margin:0 0 14px;font-size:12px;font-weight:700;letter-spacing:0.12em;text-transform:uppercase;color:#c4307a;">Delivery Address</p>
                          <table cellpadding="0" cellspacing="0" border="0">
                            <tr><td style="padding:2px 0;font-weight:700;font-size:14px;color:#2d2d2d;">{order.GuestFirstName} {order.GuestLastName}</td></tr>
                            <tr><td style="padding:2px 0;color:#555;font-size:14px;">{order.AddressLine1}</td></tr>
                            {addressLine2}
                            <tr><td style="padding:2px 0;color:#555;font-size:14px;">{order.City}, {order.Province} {order.PostalCode}</td></tr>
                          </table>
                        </td>
                      </tr>

                      <!-- ── CTA ────────────────────────────────────────────── -->
                      <tr>
                        <td style="background:#ffffff;padding:28px 40px;text-align:center;border-left:1px solid #f5e6f0;border-right:1px solid #f5e6f0;">
                          <a href="https://ladyruth.co.za/track-order/{order.OrderNumber}"
                             style="display:inline-block;background:linear-gradient(135deg,#e8468c,#c4307a);color:#ffffff;font-size:15px;font-weight:700;text-decoration:none;padding:14px 36px;border-radius:50px;letter-spacing:0.02em;">
                            Track My Order
                          </a>
                        </td>
                      </tr>

                      <!-- ── FOOTER ──────────────────────────────────────────── -->
                      <tr>
                        <td style="background:linear-gradient(135deg,#2d2d2d 0%,#1a1a1a 100%);border-radius:0 0 16px 16px;padding:32px 40px;text-align:center;">
                          <p style="margin:0 0 8px;font-size:18px;font-weight:800;color:#e8468c;letter-spacing:-0.3px;">LadyRuth</p>
                          <p style="margin:0 0 16px;font-size:12px;color:#888;">Fashion &amp; Style | South Africa</p>
                          <div style="margin:0 0 16px;height:1px;background:#333;"></div>
                          <p style="margin:0 0 6px;font-size:12px;color:#888;line-height:1.6;">
                            Questions? Email us at
                            <a href="mailto:support@ladyruth.co.za" style="color:#e8468c;text-decoration:none;">support@ladyruth.co.za</a>
                          </p>
                          <p style="margin:0;font-size:11px;color:#555;">
                            &copy; {DateTime.UtcNow.Year} LadyRuth. All rights reserved.
                          </p>
                        </td>
                      </tr>

                    </table>
                  </td>
                </tr>
              </table>

            </body>
            </html>
            """;
    }
}

using LadyRuth.API.Data;
using LadyRuth.API.DTOs.Admin;
using LadyRuth.API.DTOs.Orders;
using LadyRuth.API.DTOs.Products;
using LadyRuth.API.Entities;
using LadyRuth.API.Entities.Enums;
using PaymentStatus = LadyRuth.API.Entities.Enums.PaymentStatus;
using LadyRuth.API.Helpers;
using LadyRuth.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LadyRuth.API.Services;

public class OrderService(AppDbContext db, IConfiguration config, IEmailService emailService) : IOrderService
{
    public async Task<OrderDto> PlaceOrderAsync(PlaceOrderDto dto)
    {
        var shippingFee = config.GetValue<decimal>("ShippingFee");

        // Load and validate all variants
        var variantIds = dto.Items.Select(i => i.VariantId).ToList();
        var variants = await db.ProductVariants
            .Include(v => v.Product)
            .Where(v => variantIds.Contains(v.Id))
            .ToListAsync();

        if (variants.Count != variantIds.Count)
            throw new InvalidOperationException("One or more variants not found.");

        // Check stock
        foreach (var item in dto.Items)
        {
            var variant = variants.First(v => v.Id == item.VariantId);
            if (variant.StockQuantity < item.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for {variant.Product.Name} ({variant.Colour}, {variant.Size}). " +
                    $"Available: {variant.StockQuantity}");
        }

        // Build order items and deduct stock
        var orderItems = new List<OrderItem>();
        foreach (var item in dto.Items)
        {
            var variant = variants.First(v => v.Id == item.VariantId);
            variant.StockQuantity -= item.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductVariantId = variant.Id,
                ProductName      = variant.Product.Name,
                Colour           = variant.Colour,
                Size             = variant.Size,
                Quantity         = item.Quantity,
                UnitPrice        = variant.Product.Price
            });
        }

        var subTotal = orderItems.Sum(i => i.UnitPrice * i.Quantity);
        var total    = subTotal + shippingFee;

        var order = new Order
        {
            OrderNumber    = string.Empty,  // assigned after save
            GuestEmail     = dto.GuestEmail,
            GuestFirstName = dto.GuestFirstName,
            GuestLastName  = dto.GuestLastName,
            GuestPhone     = dto.GuestPhone,
            AddressLine1   = dto.AddressLine1,
            AddressLine2   = dto.AddressLine2,
            City           = dto.City,
            Province       = dto.Province,
            PostalCode     = dto.PostalCode,
            SubTotal       = subTotal,
            ShippingFee    = shippingFee,
            Total          = total,
            Status         = OrderStatus.Pending,
            CreatedAt      = DateTime.UtcNow,
            Items          = orderItems
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        // Assign order number using generated ID
        order.OrderNumber = OrderNumberGenerator.Generate(order.Id);
        await db.SaveChangesAsync();

        return ToDto(order);
    }

    public async Task<OrderDto?> GetByOrderNumberAsync(string orderNumber)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        return order is null ? null : ToDto(order);
    }

    public async Task<PagedResult<OrderDto>> GetAdminOrdersAsync(
        int page, int pageSize, OrderStatus? status, string? search = null)
    {
        var query = db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim().ToLower();
            query = query.Where(o =>
                o.OrderNumber.ToLower().Contains(q) ||
                o.GuestFirstName.ToLower().Contains(q) ||
                o.GuestLastName.ToLower().Contains(q) ||
                o.GuestEmail.ToLower().Contains(q));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<OrderDto>
        {
            Items      = items.Select(ToDto).ToList(),
            TotalCount = total,
            Page       = page,
            PageSize   = pageSize
        };
    }

    public async Task<OrderDto?> GetAdminOrderByIdAsync(int id)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order is null ? null : ToDto(order);
    }

    public async Task<OrderDto?> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto)
    {
        var order = await db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return null;

        var previousStatus = order.Status;
        order.Status     = dto.Status;
        order.AdminNotes = dto.AdminNotes ?? order.AdminNotes;
        order.UpdatedAt  = DateTime.UtcNow;

        await db.SaveChangesAsync();

        if (order.Status != previousStatus)
            await emailService.SendStatusUpdateAsync(ToDto(order));

        return ToDto(order);
    }

    public async Task UpdatePaymentStatusAsync(string orderNumber, string? payFastPaymentId, bool isPaid)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        if (order is null) return;

        order.PaymentStatus    = isPaid ? PaymentStatus.Complete : PaymentStatus.Failed;
        order.PayFastPaymentId = payFastPaymentId ?? order.PayFastPaymentId;
        if (isPaid) order.Status = OrderStatus.Processing;
        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        if (isPaid)
        {
            var orderDto = await db.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == order.Id);
            if (orderDto is not null)
                await emailService.SendOrderConfirmationAsync(ToDto(orderDto));
        }
    }

    private static OrderDto ToDto(Order o) => new()
    {
        Id             = o.Id,
        OrderNumber    = o.OrderNumber,
        GuestEmail     = o.GuestEmail,
        GuestFirstName = o.GuestFirstName,
        GuestLastName  = o.GuestLastName,
        GuestPhone     = o.GuestPhone,
        AddressLine1   = o.AddressLine1,
        AddressLine2   = o.AddressLine2,
        City           = o.City,
        Province       = o.Province,
        PostalCode     = o.PostalCode,
        SubTotal       = o.SubTotal,
        ShippingFee    = o.ShippingFee,
        Total          = o.Total,
        Status         = o.Status.ToString(),
        AdminNotes     = o.AdminNotes,
        CreatedAt      = o.CreatedAt,
        Items          = o.Items.Select(i => new OrderItemDto
        {
            Id          = i.Id,
            ProductName = i.ProductName,
            Colour      = i.Colour,
            Size        = i.Size,
            Quantity    = i.Quantity,
            UnitPrice   = i.UnitPrice,
            LineTotal   = i.UnitPrice * i.Quantity
        }).ToList()
    };
}

using System.ComponentModel.DataAnnotations;
using LadyRuth.API.Entities.Enums;

namespace LadyRuth.API.DTOs.Admin;

public class UpdateOrderStatusDto
{
    [Required]
    public OrderStatus Status { get; set; }

    public string? AdminNotes { get; set; }
}

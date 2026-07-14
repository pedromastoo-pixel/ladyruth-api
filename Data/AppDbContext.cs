using LadyRuth.API.Entities;
using LadyRuth.API.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace LadyRuth.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Category ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(100).IsRequired();
            e.Property(c => c.Slug).HasMaxLength(120).IsRequired();
            e.HasIndex(c => c.Slug).IsUnique();
        });

        // ── Product ───────────────────────────────────────────────────────────
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(200).IsRequired();
            e.Property(p => p.Description).HasMaxLength(2000);
            e.Property(p => p.Price).HasColumnType("decimal(10,2)");

            e.HasOne(p => p.Category)
             .WithMany(c => c.Products)
             .HasForeignKey(p => p.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── ProductImage ──────────────────────────────────────────────────────
        modelBuilder.Entity<ProductImage>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.ContentType).HasMaxLength(100).IsRequired();

            e.HasOne(i => i.Product)
             .WithMany(p => p.Images)
             .HasForeignKey(i => i.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ProductVariant ────────────────────────────────────────────────────
        modelBuilder.Entity<ProductVariant>(e =>
        {
            e.HasKey(v => v.Id);
            e.Property(v => v.Colour).HasMaxLength(50).IsRequired();
            e.Property(v => v.Size).HasMaxLength(10).IsRequired();
            e.Property(v => v.SKU).HasMaxLength(100);

            // Unique constraint: one row per product + colour + size
            e.HasIndex(v => new { v.ProductId, v.Colour, v.Size }).IsUnique();

            e.HasOne(v => v.Product)
             .WithMany(p => p.Variants)
             .HasForeignKey(v => v.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Order ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.OrderNumber).HasMaxLength(20).IsRequired();
            e.HasIndex(o => o.OrderNumber).IsUnique();
            e.Property(o => o.GuestEmail).HasMaxLength(200).IsRequired();
            e.Property(o => o.GuestFirstName).HasMaxLength(100).IsRequired();
            e.Property(o => o.GuestLastName).HasMaxLength(100).IsRequired();
            e.Property(o => o.GuestPhone).HasMaxLength(20);
            e.Property(o => o.AddressLine1).HasMaxLength(300).IsRequired();
            e.Property(o => o.AddressLine2).HasMaxLength(300);
            e.Property(o => o.City).HasMaxLength(100).IsRequired();
            e.Property(o => o.Province).HasMaxLength(100).IsRequired();
            e.Property(o => o.PostalCode).HasMaxLength(10).IsRequired();
            e.Property(o => o.SubTotal).HasColumnType("decimal(10,2)");
            e.Property(o => o.ShippingFee).HasColumnType("decimal(10,2)");
            e.Property(o => o.Total).HasColumnType("decimal(10,2)");
            e.Property(o => o.Status).HasConversion<string>();
        });

        // ── OrderItem ─────────────────────────────────────────────────────────
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
            e.Property(i => i.Colour).HasMaxLength(50).IsRequired();
            e.Property(i => i.Size).HasMaxLength(10).IsRequired();
            e.Property(i => i.UnitPrice).HasColumnType("decimal(10,2)");
            e.Ignore(i => i.LineTotal);   // computed, not stored

            e.HasOne(i => i.Order)
             .WithMany(o => o.Items)
             .HasForeignKey(i => i.OrderId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(i => i.Variant)
             .WithMany(v => v.OrderItems)
             .HasForeignKey(i => i.ProductVariantId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Testimonial ───────────────────────────────────────────────────────
        modelBuilder.Entity<Testimonial>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.CustomerName).HasMaxLength(150).IsRequired();
            e.Property(t => t.Quote).HasMaxLength(1000).IsRequired();
            e.Property(t => t.ProductName).HasMaxLength(200);
        });

        // ── AdminUser ─────────────────────────────────────────────────────────
        modelBuilder.Entity<AdminUser>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(a => a.Email).IsUnique();
            e.Property(a => a.PasswordHash).HasMaxLength(500).IsRequired();
            e.Property(a => a.Role).HasMaxLength(20).IsRequired();
        });

        // ── Seed Data ─────────────────────────────────────────────────────────
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Dresses",    Slug = "dresses",    IsActive = true },
            new Category { Id = 2, Name = "Tops",       Slug = "tops",       IsActive = true },
            new Category { Id = 3, Name = "Bottoms",    Slug = "bottoms",    IsActive = true },
            new Category { Id = 4, Name = "Accessories",Slug = "accessories",IsActive = true }
        );

        // Seed a default SuperAdmin (password: Admin@123 — change immediately!)
        modelBuilder.Entity<AdminUser>().HasData(new AdminUser
        {
            Id           = 1,
            Email        = "admin@ladyruth.co.za",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role         = "SuperAdmin",
            IsActive     = true,
            CreatedAt    = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}

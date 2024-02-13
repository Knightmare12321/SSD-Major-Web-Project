using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SSD_Major_Web_Project.ViewModels;

namespace SSD_Major_Web_Project.Models;

public partial class NovaDbContext : DbContext
{
    public NovaDbContext()
    {
    }

    public NovaDbContext(DbContextOptions<NovaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductSku> ProductSkus { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.PkDiscountCode).HasName("PK__Discount__79BE3D840D7E5502");

            entity.ToTable("Discount");

            entity.Property(e => e.PkDiscountCode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("pkDiscountCode");
            entity.Property(e => e.DiscountValue).HasColumnName("discountValue");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.PkOrderId).HasName("PK__Order__C196130B927BE545");

            entity.ToTable("Order");

            entity.HasIndex(e => e.TransactionId, "TransactionIdUnique").IsUnique();

            entity.Property(e => e.PkOrderId).HasColumnName("pkOrderId");
            entity.Property(e => e.BuyerNote)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("buyerNote");
            entity.Property(e => e.FkDiscountCode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("fkDiscountCode");
            entity.Property(e => e.FkOrderStatusId).HasColumnName("fkOrderStatusId");
            entity.Property(e => e.FkUserId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("fkUserId");
            entity.Property(e => e.OrderDate).HasColumnName("orderDate");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("transactionId");

            entity.HasOne(d => d.FkDiscountCodeNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.FkDiscountCode)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("OrderDiscountFK");

            entity.HasOne(d => d.FkOrderStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.FkOrderStatusId)
                .HasConstraintName("OrderOrderStatusFK");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Orders)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("OrderUserFK");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.FkOrderId, e.FkSkuId }).HasName("PK__OrderDet__1F82522D00DE1B48");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.FkOrderId).HasColumnName("fkOrderId");
            entity.Property(e => e.FkSkuId).HasColumnName("fkSkuId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.FkOrder).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.FkOrderId)
                .HasConstraintName("OrderDetailOrderFK");

            entity.HasOne(d => d.FkSku).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.FkSkuId)
                .HasConstraintName("OrderDetailProductSkuFK");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.PkOrderStatusId).HasName("PK__OrderSta__ABDB68871A1B5F70");

            entity.ToTable("OrderStatus");

            entity.Property(e => e.PkOrderStatusId).HasColumnName("pkOrderStatusId");
            entity.Property(e => e.OrderStatus1)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("orderStatus");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.PkProductId).HasName("PK__Product__4492A4B592F629A3");

            entity.ToTable("Product");

            entity.Property(e => e.PkProductId).HasColumnName("pkProductId");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Image)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("image");
            entity.Property(e => e.IsActive)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
        });

        modelBuilder.Entity<ProductSku>(entity =>
        {
            entity.HasKey(e => e.PkSkuId).HasName("PK__ProductS__B7ADEE3BB01B0291");

            entity.ToTable("ProductSku");

            entity.Property(e => e.PkSkuId).HasColumnName("pkSkuId");
            entity.Property(e => e.FKproductId).HasColumnName("fKProductId");
            entity.Property(e => e.Size)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("size");

            entity.HasOne(d => d.FKproduct).WithMany(p => p.ProductSkus)
                .HasForeignKey(d => d.FKproductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ProductSkuProductFK");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => new { e.FkUserId, e.FkProductId }).HasName("PK__Review__465188073B545504");

            entity.ToTable("Review");

            entity.Property(e => e.FkUserId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("fkUserId");
            entity.Property(e => e.FkProductId).HasColumnName("fkProductId");
            entity.Property(e => e.Comment)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("comment");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.FkProduct).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.FkProductId)
                .HasConstraintName("ReviewProductFK");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("ReviewUserFK");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.PkUserId).HasName("PK__User__1790FCDFAA5A2562");

            entity.ToTable("User");

            entity.Property(e => e.PkUserId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("pkUserId");
            entity.Property(e => e.Address)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Address2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("address2");
            entity.Property(e => e.City)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("country");
            entity.Property(e => e.FirstName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("firstName");
            entity.Property(e => e.FkUserTypeId).HasColumnName("fkUserTypeId");
            entity.Property(e => e.LastName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("lastName");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phoneNumber");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("postalCode");
            entity.Property(e => e.Province)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("province");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public DbSet<SSD_Major_Web_Project.ViewModels.CreateProductVM> CreateProductVM { get; set; } = default!;
}

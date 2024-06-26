﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductSku> ProductSkus { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.PkContactId).HasName("PK__Contact__5D8CD4C06595E06D");

            entity.ToTable("Contact");

            entity.Property(e => e.PkContactId).HasColumnName("pkContactId");
            entity.Property(e => e.Address)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Address2)
                .HasMaxLength(30)
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

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.PkCustomerId).HasName("PK__Customer__1FD9D5A20CB69903");

            entity.ToTable("Customer");

            entity.Property(e => e.PkCustomerId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("pkCustomerId");
            entity.Property(e => e.FkContactId).HasColumnName("fkContactId");

            entity.HasOne(d => d.FkContact).WithMany(p => p.Customers)
                .HasForeignKey(d => d.FkContactId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("CustomerContactFK");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.PkDiscountCode).HasName("PK__Discount__79BE3D84BAAFCA85");

            entity.ToTable("Discount");

            entity.Property(e => e.PkDiscountCode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("pkDiscountCode");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("discountType");
            entity.Property(e => e.DiscountValue)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("discountValue");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.PkImageId).HasName("PK__Image__03FC4376632796A1");

            entity.ToTable("Image");

            entity.Property(e => e.PkImageId).HasColumnName("pkImageId");
            entity.Property(e => e.AltText)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("altText");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.FileName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("fileName");
            entity.Property(e => e.FkProductId).HasColumnName("fkProductId");

            entity.HasOne(d => d.FkProduct).WithMany(p => p.Images)
                .HasForeignKey(d => d.FkProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ImageProductFK");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.PkOrderId).HasName("PK__Order__C196130B260D274E");

            entity.ToTable("Order");

            entity.HasIndex(e => e.TransactionId, "TransactionIdUnique").IsUnique();

            entity.Property(e => e.PkOrderId).HasColumnName("pkOrderId");
            entity.Property(e => e.BuyerNote)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("buyerNote");
            entity.Property(e => e.FkContactId).HasColumnName("fkContactId");
            entity.Property(e => e.FkCustomerId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("fkCustomerId");
            entity.Property(e => e.FkDiscountCode)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("fkDiscountCode");
            entity.Property(e => e.FkOrderStatusId).HasColumnName("fkOrderStatusId");
            entity.Property(e => e.OrderDate).HasColumnName("orderDate");
            entity.Property(e => e.ShipDate).HasColumnName("shipDate");
            entity.Property(e => e.Tracking).HasColumnName("tracking");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("transactionId");

            entity.HasOne(d => d.FkContact).WithMany(p => p.Orders)
                .HasForeignKey(d => d.FkContactId)
                .HasConstraintName("OrderAddressFK");

            entity.HasOne(d => d.FkCustomer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.FkCustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("OrderCustomerFK");

            entity.HasOne(d => d.FkDiscountCodeNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.FkDiscountCode)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("OrderDiscountFK");

            entity.HasOne(d => d.FkOrderStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.FkOrderStatusId)
                .HasConstraintName("OrderOrderStatusFK");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.FkOrderId, e.FkSkuId }).HasName("PK__OrderDet__1F82522D37E449E5");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.FkOrderId).HasColumnName("fkOrderId");
            entity.Property(e => e.FkSkuId).HasColumnName("fkSkuId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unitPrice");

            entity.HasOne(d => d.FkOrder).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.FkOrderId)
                .HasConstraintName("OrderDetailOrderFK");

            entity.HasOne(d => d.FkSku).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.FkSkuId)
                .HasConstraintName("OrderDetailProductSkuFK");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.PkOrderStatusId).HasName("PK__OrderSta__ABDB6887C008B250");

            entity.ToTable("OrderStatus");

            entity.Property(e => e.PkOrderStatusId).HasColumnName("pkOrderStatusId");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.PkProductId).HasName("PK__Product__4492A4B5D1C26E1C");

            entity.ToTable("Product");

            entity.Property(e => e.PkProductId).HasColumnName("pkProductId");
            entity.Property(e => e.Description)
                .HasMaxLength(5000)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<ProductSku>(entity =>
        {
            entity.HasKey(e => e.PkSkuId).HasName("PK__ProductS__B7ADEE3BE27B8D64");

            entity.ToTable("ProductSku");

            entity.Property(e => e.PkSkuId).HasColumnName("pkSkuId");
            entity.Property(e => e.FkProductId).HasColumnName("fkProductId");
            entity.Property(e => e.Size)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("size");

            entity.HasOne(d => d.FkProduct).WithMany(p => p.ProductSkus)
                .HasForeignKey(d => d.FkProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("ProductSkuProductFK");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => new { e.FkCustomerId, e.FkProductId, e.PkReviewDate }).HasName("PK__Review__D33188289843F8BE");

            entity.ToTable("Review");

            entity.Property(e => e.FkCustomerId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("fkCustomerId");
            entity.Property(e => e.FkProductId).HasColumnName("fkProductId");
            entity.Property(e => e.PkReviewDate).HasColumnName("pkReviewDate");
            entity.Property(e => e.Comment)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("comment");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.FkCustomer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.FkCustomerId)
                .HasConstraintName("ReviewCustomerFK");

            entity.HasOne(d => d.FkProduct).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.FkProductId)
                .HasConstraintName("ReviewProductFK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

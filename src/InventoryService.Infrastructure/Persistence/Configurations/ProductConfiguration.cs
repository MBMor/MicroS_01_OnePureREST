using InventoryService.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryService.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(product => product.Name)
            .HasColumnName("name")
            .HasMaxLength(ProductConstraints.NameMaxLength)
            .IsRequired();

        builder.Property(product => product.Description)
            .HasColumnName("description")
            .HasMaxLength(ProductConstraints.DescriptionMaxLength);

        builder.Property(product => product.Sku)
            .HasColumnName("sku")
            .HasMaxLength(ProductConstraints.SkuMaxLength)
            .IsRequired();

        builder.Property(product => product.Price)
            .HasColumnName("price")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(product => product.QuantityInStock)
            .HasColumnName("quantity_in_stock")
            .IsRequired();

        builder.Property(product => product.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(product => product.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(product => product.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(product => product.Sku)
            .IsUnique()
            .HasDatabaseName("ux_products_sku");
    }
}
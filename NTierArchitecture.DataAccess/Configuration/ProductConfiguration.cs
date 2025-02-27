using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NTierArchitecture.Entities.Models;

namespace NTierArchitecture.DataAccess.Configuration;
internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(c => c.Id);
        builder.Property(p => p.Price).HasColumnType("money");
    }
}

using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESM.Infrastructure.Data.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.HasKey(ii => ii.Id);

        builder.Property(ii => ii.ServiceName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ii => ii.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(ii => ii.Quantity)
            .IsRequired();

        builder.HasQueryFilter(ii => !ii.IsDeleted);

        builder.HasOne(ii => ii.Service)
            .WithMany(s => s.InvoiceItems)
            .HasForeignKey(ii => ii.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

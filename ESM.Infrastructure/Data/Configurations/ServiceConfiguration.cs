using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESM.Infrastructure.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.BasePrice)
            .HasColumnType("decimal(18,2)");

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasMany(s => s.ServiceRequests)
            .WithOne(sr => sr.Service)
            .HasForeignKey(sr => sr.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.InvoiceItems)
            .WithOne(ii => ii.Service)
            .HasForeignKey(ii => ii.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

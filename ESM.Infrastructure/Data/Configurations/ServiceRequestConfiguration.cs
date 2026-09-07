using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESM.Infrastructure.Data.Configurations;

public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
    public void Configure(EntityTypeBuilder<ServiceRequest> builder)
    {
        builder.HasKey(sr => sr.Id);

        builder.Property(sr => sr.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(sr => sr.ServiceLocation)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasQueryFilter(sr => !sr.IsDeleted);

        builder.HasOne(sr => sr.Organization)
            .WithMany(o => o.ServiceRequests)
            .HasForeignKey(sr => sr.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.Customer)
            .WithMany(c => c.ServiceRequests)
            .HasForeignKey(sr => sr.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.Service)
            .WithMany(s => s.ServiceRequests)
            .HasForeignKey(sr => sr.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sr => sr.CreatedByUser)
            .WithMany(u => u.ServiceRequests)
            .HasForeignKey(sr => sr.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional Appointment and Review relationships are configured in their respective classes
    }
}

using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESM.Infrastructure.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.StartDate)
            .IsRequired();

        builder.Property(a => a.EndDate)
            .IsRequired();

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.HasOne(a => a.ServiceRequest)
            .WithOne(sr => sr.Appointment)
            .HasForeignKey<Appointment>(a => a.ServiceRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Employee)
            .WithMany(e => e.Appointments)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional Invoice is configured in InvoiceConfiguration
    }
}

using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESM.Infrastructure.Data.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.NameAr).IsRequired().HasMaxLength(200);
        builder.Property(o => o.NameEn).IsRequired().HasMaxLength(200);

        builder.HasQueryFilter(o => !o.IsDeleted);

        builder.HasMany(o => o.Users)
               .WithOne(u => u.Organization)
               .HasForeignKey(u => u.OrganizationId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Customers)
               .WithOne(c => c.Organization)
               .HasForeignKey(c => c.OrganizationId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Departments)
               .WithOne(d => d.Organization)
               .HasForeignKey(d => d.OrganizationId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Services)
               .WithOne(s => s.Organization)
               .HasForeignKey(s => s.OrganizationId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.ServiceRequests)
               .WithOne(sr => sr.Organization)
               .HasForeignKey(sr => sr.OrganizationId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

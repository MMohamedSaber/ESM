using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESM.Infrastructure.Data.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Description).HasMaxLength(1000);
        builder.Property(o => o.Email).HasMaxLength(256);
        builder.Property(o => o.Phone).HasMaxLength(50);
        builder.Property(o => o.Address).HasMaxLength(500);

        builder.HasQueryFilter(o => !o.IsDeleted);

        builder.HasMany(o => o.Departments)
               .WithOne(d => d.Organization)
               .HasForeignKey(d => d.OrganizationId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Employees)
               .WithOne(e => e.Organization)
               .HasForeignKey(e => e.OrganizationId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

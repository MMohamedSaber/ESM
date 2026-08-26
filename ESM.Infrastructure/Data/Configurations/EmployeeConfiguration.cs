using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESM.Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.UserId).IsRequired().HasMaxLength(450); // Match IdentityUser Id length
        builder.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Position).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Salary).HasColumnType("decimal(18,2)");

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

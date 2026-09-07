using ESM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESM.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Comments)
            .HasMaxLength(1000);

        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.HasOne(r => r.ServiceRequest)
            .WithOne(sr => sr.Review)
            .HasForeignKey<Review>(r => r.ServiceRequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

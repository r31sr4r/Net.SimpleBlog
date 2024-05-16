using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.Infra.Data.EF.Configurations;
internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(user => user.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(user => user.CPF)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(user => user.RG)
            .HasMaxLength(20);

        builder.Property(user => user.DateOfBirth)
            .IsRequired();

        builder.Property(user => user.Password)
            .HasMaxLength(1000);

        builder.Property(user => user.IsActive)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.AnalysisDate)
            .HasColumnType("datetime(6)");

    }
}

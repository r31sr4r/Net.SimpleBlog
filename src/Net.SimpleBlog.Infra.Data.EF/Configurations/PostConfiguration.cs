using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.Infra.Data.EF.Configurations
{
    internal class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(post => post.Id);

            builder.Property(post => post.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(post => post.Content)
                .IsRequired();

            builder.Property(post => post.UserId)
                .IsRequired();

            builder.Property(post => post.CreatedAt)
                .IsRequired();

            builder.Property(post => post.UpdatedAt)
                .HasColumnType("datetime(6)");

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(post => post.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

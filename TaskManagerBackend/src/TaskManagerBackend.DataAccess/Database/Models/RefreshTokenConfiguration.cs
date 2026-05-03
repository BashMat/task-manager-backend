using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManagerBackend.DataAccess.Database.Models;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(e => e.Id).HasName("RefreshToken_PK");

        builder.ToTable("RefreshToken");

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasOne(d => d.User)
               .WithMany(p => p.RefreshToken)
               .HasForeignKey(d => d.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("RefreshToken_UserId_FK")
               .IsRequired();
    }
}
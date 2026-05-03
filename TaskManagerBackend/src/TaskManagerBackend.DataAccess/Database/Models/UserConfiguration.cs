using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManagerBackend.DataAccess.Database.Models;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id).HasName("User_PK");

        builder.ToTable("User");

        builder.Property(e => e.Email).HasMaxLength(256);
        builder.Property(e => e.FirstName).HasMaxLength(256);
        builder.Property(e => e.LastName).HasMaxLength(256);
        builder.Property(e => e.PasswordHash).HasMaxLength(256);
        builder.Property(e => e.PasswordSalt).HasMaxLength(256);
        builder.Property(e => e.UserName).HasMaxLength(256);
    }
}
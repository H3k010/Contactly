using ContactsManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactsManager.Infrastructure.Data.Config;

public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.OwnsOne(u => u.UserPreferences, p =>
        {
            p.Property(pref => pref.PageSize).HasConversion<int>();
            p.Property(pref => pref.SortBy).HasConversion<string>().HasMaxLength(50);
            p.Property(pref => pref.SortOrder).HasConversion<string>().HasMaxLength(15);
        });
        
        builder.Property(c => c.RegisteredAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
    }
}

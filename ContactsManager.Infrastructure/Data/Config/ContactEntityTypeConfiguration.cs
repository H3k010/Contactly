using ContactsManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactsManager.Infrastructure.Data.Config;

public class ContactEntityTypeConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts").HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .HasColumnType("CHAR(36)")
            .ValueGeneratedOnAdd();
        
        builder.Property(c => c.FirstName)
            .HasColumnType("VARCHAR(50)");
        
        builder.Property(c => c.LastName)
            .HasColumnType("VARCHAR(50)");
        
        builder.Property(c => c.PhoneNumber)
            .HasColumnType("VARCHAR(20)");
        
        builder.Property(c => c.Address)
            .HasColumnType("VARCHAR(255)");
        
        builder.Property(c => c.Email)
            .HasColumnType("VARCHAR(100)");
        
        builder.Property(c => c.CompanyName)
            .HasColumnType("VARCHAR(100)");
        
        builder.Property(c => c.Notes)
            .HasColumnType("VARCHAR(255)");
        
        
        builder.HasOne(c => c.User)
            .WithMany(u => u.Contacts)
            .HasForeignKey(c => c.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.CreatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.UpdatedAt)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();
    }
}
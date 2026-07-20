using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccess.Configurations
{
    internal class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            // Таблица
            builder.ToTable("persons");

            builder.HasKey(a => a.Id);
            builder.HasIndex(k => k.Login)
                .IsUnique();

            builder.Property(p => p.Login)
                .IsRequired();

            builder.Property(p => p.PasswordHash)
                .IsRequired();

            builder.Property(p => p.Role)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion(v => v.ToString().ToLower(), v => Enum.Parse<PersonRole>(v, true));
        }
    }
}

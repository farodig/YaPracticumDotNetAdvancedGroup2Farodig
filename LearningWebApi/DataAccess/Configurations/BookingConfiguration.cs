using LearningWebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningWebApi.DataAccess.Configurations
{
    internal class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            // Таблица
            builder.ToTable("bookings");

            // Первичный ключ
            builder.HasKey(a => a.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            // Храним Status как строку
            builder.Property(b => b.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion(v => v.ToString().ToLower(), v => Enum.Parse<BookingStatus>(v, true));

            // Связь с Event
            builder.HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId);
        }
    }
}

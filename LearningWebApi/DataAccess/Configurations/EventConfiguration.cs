using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningWebApi.DataAccess.Configurations
{
    internal class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            // Таблица
            builder.ToTable("events", t =>
            {
                // Ограничения даты и времени
                t.HasCheckConstraint("CK_Event_Dates", "\"StartAt\" < \"EndAt\"");
                t.HasCheckConstraint("CK_Event_StartAt", "\"StartAt\" > NOW()");
                t.HasCheckConstraint("CK_Event_TotalSeats", "\"TotalSeats\" >= 1");
            });

            // Первичный ключ
            builder.HasKey(a => a.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            // Ограничения свойств
            builder.Property(e => e.Title)
                   .IsRequired();

            builder.Property(e => e.StartAt)
                   .IsRequired();

            builder.Property(e => e.EndAt)
                   .IsRequired();

            builder.Property(e => e.TotalSeats)
                .IsRequired();
        }
    }
}

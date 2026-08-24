using EventService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventService.Infrastructure.DataAccess
{
    internal class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
    {
        public void Configure(EntityTypeBuilder<InboxMessage> builder)
        {
            // Таблица
            builder.ToTable("inbox_messages");

            // Первичный ключ
            builder.HasKey(a => a.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedNever();
        }
    }
}

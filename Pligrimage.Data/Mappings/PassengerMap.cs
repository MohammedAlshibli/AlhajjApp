using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;

namespace Pligrimage.Data.Mappings
{
    public class PassengerMap : IEntityTypeConfiguration<Passenger>
    {
        public void Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder.HasKey(c => c.PassengerId);

            builder.HasOne(c => c.AlhajjMaster)
                .WithMany(c => c.Passengers)
                .HasForeignKey(c => c.PligrimageId);

            builder.HasOne(c => c.Flight)
                .WithMany(c => c.Passenger)
                .HasForeignKey(c => c.FlightId);

            builder.HasOne(c => c.Buses)
                .WithMany()
                .HasForeignKey(c => c.BusId);

            // Index for fast tenant-scoped queries
            builder.HasIndex(p => new { p.TenantId, p.AlhajYear });

            builder.Property(c => c.TenantId).IsRequired();
        }
    }
}

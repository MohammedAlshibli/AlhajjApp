using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;


namespace Pligrimage.Data.Mappings
{
    public class BusesMap : IEntityTypeConfiguration<Buses>
    {
        public void Configure(EntityTypeBuilder<Buses> builder)
        {
            builder.HasKey(c => c.BusId);
            builder.HasOne(c => c.flight)
                .WithMany(c => c.buses)
                .HasForeignKey(c => c.FlightId);
        }

    }
}

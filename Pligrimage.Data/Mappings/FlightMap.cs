using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;

namespace Pligrimage.Data.Mappings
{
   public  class FlightMap : IEntityTypeConfiguration<Flight>
    {
        public void Configure(EntityTypeBuilder<Flight> builder)
        {
            builder.HasKey(c => c.FlightId);

            builder.HasOne(c => c.Parameter)
                  .WithMany(c => c.Flights)
                  .HasForeignKey(c => c.ParameterId);

        }
    }



}

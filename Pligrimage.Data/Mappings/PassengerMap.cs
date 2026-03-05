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


            builder.HasOne(c => c.Flight)
                .WithMany(c => c.Passenger)
                .HasForeignKey(c => c.FlightId);

            builder.HasOne(c => c.Buses)
                .WithMany(c => c.Passenger)
                .HasForeignKey(c => c.BusId);

            //builder.HasOne(c => c.PassengerSupervisors)
            //    .WithMany(c => c.Passenger)
            //    .HasForeignKey(c => c.PassengerSuppId);

            builder.HasOne(p => p.AlhajjMaster)
                .WithMany(p => p.Passengers);
                

            //builder.HasIndex(p => new { p.PligrimageId, p.AlhajYear }).IsUnique();

        }
    }
}

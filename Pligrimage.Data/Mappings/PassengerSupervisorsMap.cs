using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;

namespace Pligrimage.Data.Mappings
{
  public  class PassengerSupervisorsMap : IEntityTypeConfiguration<PassengerSupervisors>
    {
        public void Configure(EntityTypeBuilder<PassengerSupervisors> builder)
        {
            builder.HasKey(c => c.PassengerSuppId);

            builder.HasOne(c => c.AlhajjMaster)
                .WithMany(c => c.PassengerSupervisors)
                .HasForeignKey(c => c.PassengerSuppId);

        }
    }
}

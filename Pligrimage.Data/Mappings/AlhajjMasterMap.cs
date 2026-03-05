using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;


namespace Pligrimage.Data.Mappings
{
  public  class AlhajjMasterMap : IEntityTypeConfiguration<AlhajjMaster>
    { 
        public void Configure(EntityTypeBuilder<AlhajjMaster> builder)
        {
            builder.HasKey(c => c.PligrimageId);

            builder.HasOne(c => c.Category)
                .WithMany(c => c.AlhajjMasters)
                .HasForeignKey(c => c.CategoryId);

            builder.HasOne(c => c.Unit)
                .WithMany(c => c.AlhajjMasters)
                .HasForeignKey(c => c.UnitId);

            builder.HasOne(c => c.Document)
                .WithMany(c => c.AlhajjMasters)
                .HasForeignKey(c => c.DocumentId);

            builder.HasOne(c => c.Parameter)
                .WithMany(c => c.AlhajjMasters)
                .HasForeignKey(c => c.ParameterId);


            builder.HasIndex(p => new { p.NIC,}).IsUnique();

        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;

namespace Pligrimage.Data.Mappings
{
    public class AlhajjMasterMap : IEntityTypeConfiguration<AlhajjMaster>
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

            // BUG-FIX: NIC must be unique per tenant per year only.
            // The old global unique index blocked the same person from
            // registering in a different Hajj year.
            // New constraint: (TenantId, NIC, AlhajYear) — unique combination.
            builder.HasIndex(p => new { p.TenantId, p.NIC, p.AlhajYear })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0"); // only enforce on active records

            // TenantId is required — every pilgrim must belong to a unit
            builder.Property(c => c.TenantId).IsRequired();
        }
    }
}

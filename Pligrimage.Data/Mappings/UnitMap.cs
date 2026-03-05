using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Data.Mappings
{
   public class UnitMap : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.HasKey(c => c.UnitId);
        }
    }
}

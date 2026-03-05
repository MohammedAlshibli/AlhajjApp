using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Data.Mappings
{
    public class RoleMap : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}

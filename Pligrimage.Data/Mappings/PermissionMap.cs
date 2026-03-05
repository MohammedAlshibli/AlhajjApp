using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Data.Mappings
{
  public  class PermissionMap : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasIndex(c => c.PermissionId).IsUnique();
        }
    }
}

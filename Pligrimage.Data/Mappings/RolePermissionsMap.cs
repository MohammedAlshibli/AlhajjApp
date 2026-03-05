using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Data.Mappings
{
   public  class RolePermissionsMap : IEntityTypeConfiguration<RolePermissions>
    {

        public void Configure(EntityTypeBuilder<RolePermissions> builder)
        {
            builder.HasKey(c => new { c.RoleId, c.PermissionId });

            builder.HasOne(c => c.Role)
                .WithMany(c => c.RolePermissions)
                .HasForeignKey(c => c.RoleId);

            builder.HasOne(c => c.Permission)
                   .WithMany(c => c.RolePermissions)
                   .HasForeignKey(c => c.PermissionId);


        }

    }
}

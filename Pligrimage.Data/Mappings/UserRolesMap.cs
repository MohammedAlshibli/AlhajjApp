using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Data.Mappings
{
   public  class UserRolesMap : IEntityTypeConfiguration<UserRoles>
    {
        public void Configure(EntityTypeBuilder<UserRoles> builder)
        {
            builder.HasKey(c => new { c.UserId, c.RoleId });

            builder.HasOne(c => c.User)
                .WithMany(c => c.UserRoles)
                .HasForeignKey(c => c.UserId);

            builder.HasOne(c => c.Role)
                .WithMany(c => c.UserRoles)
                .HasForeignKey(c => c.RoleId);


        }
    }
}

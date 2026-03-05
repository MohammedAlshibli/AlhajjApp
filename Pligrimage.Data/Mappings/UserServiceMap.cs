using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities.IdentityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Data.Mappings
{
    public class UserServiceMap : IEntityTypeConfiguration<UserService>
    {
        public void Configure(EntityTypeBuilder<UserService> builder)
        {
            builder.HasKey(c => new { c.UserId, c.ServiceId });

            builder.HasOne(c => c.User)
                .WithMany(c => c.UserServices)
                .HasForeignKey(c => c.UserId);

            builder.HasOne(c => c.Unit)
                .WithMany(c => c.UserService)
                .HasForeignKey(c => c.ServiceId);
 

        }
    }
}


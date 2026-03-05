using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using System;
using System.Collections.Generic;
using System.Text;


namespace Pligrimage.Data.Mappings
{
  public   class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
             builder.HasIndex(c => c.UserName).IsUnique();
        }
    }
}

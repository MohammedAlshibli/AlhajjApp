using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pligrimage.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Data.Mappings
{
   public  class ParameterMap : IEntityTypeConfiguration<Parameter>
    
    {
        public void Configure(EntityTypeBuilder<Parameter> builder)
        {
            builder.HasKey(c => c.ParameterId);
        }


    }
}

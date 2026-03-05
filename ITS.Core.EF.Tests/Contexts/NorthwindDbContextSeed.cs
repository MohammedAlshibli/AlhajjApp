using System.Collections.Generic;
using ITS.Core.EF.Tests.Contexts;
using ITS.Core.EF.Tests.Models;
using Microsoft.EntityFrameworkCore;

namespace ITS.Core.EF.Tests.Contexts
{
    internal static class NorthwindDbContextSeed
    {
        public static void SeedDataSql(this NorthwindDbContext context,
            IEnumerable<Category> categories, IEnumerable<Product> products)
        {
            try
            {
                context.Database.OpenConnection();
                context.Database.ExecuteSqlCommand("DELETE Categories");
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Categories OFF");
                context.Categories.AddRange(categories);
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Categories OFF");

                // context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Categories ON");
                context.SaveChanges();

                context.Database.ExecuteSqlCommand("DELETE Products");
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Products OFF");

                context.Products.AddRange(products);
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Products OFF");

                //  context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Products ON");
                context.SaveChanges();
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }
    }
}
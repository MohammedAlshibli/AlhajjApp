using ITS.Core.Abstractions.Trackable;
using ITS.Core.Services;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Data;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pligrimage.Services.Implementation
{
    public class AlHajjMasterServcieTst : Service<AlhajjMaster>, IAlHajjMasterServcieTst
    {
        private readonly AppDbContext _context;

        public AlHajjMasterServcieTst(AppDbContext context, ITrackableRepository<AlhajjMaster> repository) : base(repository)
        {
            _context = context;
        }

        public async Task<List<AlhajjMaster>> GetAllData()
        {
            var list = _context.AlhajjMasters.Include(a => a.Category).Include(a => a.Document).Include(a => a.Parameter).Include(a => a.Unit).ToList();

            return list;
        }
    }
}

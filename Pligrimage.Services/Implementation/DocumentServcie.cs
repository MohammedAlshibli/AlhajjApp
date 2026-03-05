using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Services;
using ITS.Core.Abstractions.Trackable;
using ITS.Core.Services;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;

namespace Pligrimage.Services.Implementation
{
   public  class DocumentServcie : Service<Document>,IDocumentServcie
    {

        public DocumentServcie(ITrackableRepository<Document> documentRepository): base(documentRepository)
        {

        }

        public async Task<IEnumerable<Document>> ByDocumnetTypeAsync(string DocumnetType)
        {
            return await Repository.Queryable().Where(c => c.DocumnetType == DocumnetType).ToListAsync();
        }
    }
}

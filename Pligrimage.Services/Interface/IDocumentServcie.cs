using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Services;
using Pligrimage.Entities;

namespace Pligrimage.Services.Interface
{
   public  interface IDocumentServcie : IService<Document>
    {
        Task<IEnumerable<Document>> ByDocumnetTypeAsync(string DocumnetType);
    }
}

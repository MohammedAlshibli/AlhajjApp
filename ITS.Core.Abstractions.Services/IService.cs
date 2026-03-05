

using ITS.Core.Abstractions.Trackable;
using ITS.TrackableEntities.Common.Core;

namespace ITS.Core.Abstractions.Services
{
    public interface IService<TEntity>: ITrackableRepository<TEntity> where TEntity : class, ITrackable
    {
    }
}

using ITS.Core.Abstractions.Trackable;
using ITS.Core.Services;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Common.ViewModel;

namespace Pligrimage.Services.Implementation
{
   public class PermissionService : Service<Permission>, IPermissionService
    {
        public PermissionService(ITrackableRepository<Permission> PermissionRepository) : base(PermissionRepository)
        {

        }


    }
}

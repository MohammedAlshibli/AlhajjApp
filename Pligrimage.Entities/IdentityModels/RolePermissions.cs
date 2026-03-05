using ITS.Core.EF.Trackable;

namespace Pligrimage.Entities.IdentityModels
{
    public class RolePermissions: Entity
    {

        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }


    }
}
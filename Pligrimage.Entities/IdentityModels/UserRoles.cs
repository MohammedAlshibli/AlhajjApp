

using ITS.Core.EF.Trackable;

namespace Pligrimage.Entities.IdentityModels
{
    public class UserRoles : Entity
    {

        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }


    }
}

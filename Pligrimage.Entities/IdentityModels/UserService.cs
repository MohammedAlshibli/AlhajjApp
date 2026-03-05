

using ITS.Core.EF.Trackable;

namespace Pligrimage.Entities.IdentityModels
{
    public class UserService : Entity
    {

        public int UserId { get; set; }
        public int ServiceId { get; set; }

        public virtual User User { get; set; }
        public virtual Unit Unit { get; set; }


    }
}

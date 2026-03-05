using ITS.Core.EF.Trackable;
using System;

namespace Pligrimage.Entities
{
    public abstract class BaseEntity : Entity
    {
        public DateTime? CreateOn  { get; set; } = DateTime.Now;
        public string   CreateBy  { get; set; } = PligrimageConstants.UserName;
        public DateTime? UpdatedOn { get; set; }
        public string   UpdatedBy { get; set; }

        // ── Soft delete ───────────────────────────────────────────────────
        /// <summary>
        /// True = record is cancelled/deleted. Never hard-delete pilgrim records.
        /// </summary>
        public bool     IsDeleted  { get; set; } = false;
        public DateTime? DeletedOn { get; set; }
        public string   DeletedBy  { get; set; }
    }
}

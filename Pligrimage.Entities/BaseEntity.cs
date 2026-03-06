using ITS.Core.EF.Trackable;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pligrimage.Entities
{
    public abstract class BaseEntity : Entity
    {
        // ── Audit ─────────────────────────────────────────────────────────
        public DateTime? CreateOn  { get; set; } = DateTime.Now;
        public string   CreateBy  { get; set; } = PligrimageConstants.UserName;
        public DateTime? UpdatedOn { get; set; }
        public string   UpdatedBy { get; set; }

        // ── Soft delete ───────────────────────────────────────────────────
        public bool     IsDeleted  { get; set; } = false;
        public DateTime? DeletedOn { get; set; }
        public string   DeletedBy  { get; set; }

        // ── Multi-Tenancy ─────────────────────────────────────────────────
        /// <summary>
        /// Maps to Units.UnitId.
        /// Every record created is automatically stamped with the
        /// current user's unit (tenant). The DbContext Global Query Filter
        /// enforces this on every query — no manual .Where() needed.
        /// SysAdmins (TenantId == 0) bypass the filter and see all data.
        /// </summary>
        public int TenantId { get; set; }

        [NotMapped]
        public static readonly int SysAdminTenantId = 0;
    }
}

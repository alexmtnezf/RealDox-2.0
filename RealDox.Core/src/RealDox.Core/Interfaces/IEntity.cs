using System;
using System.ComponentModel.DataAnnotations;

namespace RealDox.Core.Interfaces
{
    /// <summary>
    /// This interface should be implemented by all domain entities.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The unique identifier for this entity.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// UTC date time of entity creation.
        /// </summary>
        [ScaffoldColumn(false)]
        DateTime CreatedUtc { get; }

        [ScaffoldColumn(false)]
        string IPAddressAdded { get; }

        /// <summary>
        /// The identifier for the user who has created this entity.
        /// </summary>
        [ScaffoldColumn(false)]
        string CreatedBy { get; set; }

        /// <summary>
        /// UTC date time of entity update.
        /// </summary>
        [ScaffoldColumn(false)]
        DateTime? LastUpdatedUtc { get; set; }

        [ScaffoldColumn(false)]
        string IPAddressUpdated { get; set; }

        /// <summary>
        /// The identifier for the user who has updated this entity last time.
        /// </summary>
        [ScaffoldColumn(false)]
        string LastUpdatedBy { get; set;}

        [ScaffoldColumn(false)]
        DateTime? DeleteDate { get; set; }

        [ScaffoldColumn(false)]
        string DeletedBy { get; set; }

        [ScaffoldColumn(false)]
        string IPAddressDeleted { get; set; }

        [ScaffoldColumn(false)]
        bool Deleted { get; set; }

        [ScaffoldColumn(false)]
        byte[] RowVersion { get; set; }

        /// <summary>
        /// Checks if this entity is transient (not persisted to database and it has not an valid ID).
        /// </summary>
        /// <returns>True, if this entity is transient; otherwise false.</returns>
        bool IsTransient();

        /// <summary>
        /// Generates identity for this entity.
        /// </summary>
        void GenerateIdentity();
    }
}
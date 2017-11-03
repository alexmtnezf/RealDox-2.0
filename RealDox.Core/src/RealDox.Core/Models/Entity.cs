using System;
using System.ComponentModel.DataAnnotations;
using RealDox.Core.Interfaces;

namespace RealDox.Core.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Basic implementation of IEntity interface.
    /// An entity can inherit this class of directly implement to IEntity interface.
    /// </summary>
    public abstract class Entity : IEntity
    {
        #region Properties

        /// <inheritdoc />        
        [StringLength(256)]
        public virtual string Id { get; protected set; }

        /// <inheritdoc />
        public DateTime CreatedUtc { get; set; }

        /// <inheritdoc />
        [StringLength(256)]
        public string CreatedBy { get; set; }

        /// <inheritdoc />
        public DateTime? LastUpdatedUtc { get; set; }

        /// <inheritdoc />
        [StringLength(256)]
        public string LastUpdatedBy { get; set; }
        /// <inheritdoc />
        public string IPAddressAdded { get; set;}
        /// <inheritdoc />
        public string IPAddressUpdated { get; set;}
        /// <inheritdoc />
        public DateTime? DeleteDate { get; set;}
        /// <inheritdoc />
        public string DeletedBy { get; set;}
        /// <inheritdoc />
        public string IPAddressDeleted { get; set;}
        /// <inheritdoc />
        public bool Deleted { get; set;}
        /// <inheritdoc />
        public byte[] RowVersion { get; set;}

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public virtual bool IsTransient()
        {
            return string.IsNullOrEmpty(Id);
        }

        /// <inheritdoc />
        public virtual void GenerateIdentity()
        {
            // We should not re-produce an identity for a valid entity.
            if (!IsTransient()) return;

            Id = Guid.NewGuid().ToString();
            CreatedUtc = DateTime.UtcNow;
        }

        #endregion

        #region Overrides Methods

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Entity))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (Entity)obj;

            if (item.IsTransient() || IsTransient())
                return false;

            return item.Id == Id;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{GetType().Name} [{Id}]";
        }

        /// <inheritdoc />
        public static bool operator ==(Entity left, Entity right)
        {
            return Equals(left, null) ? Equals(right, null) : left.Equals(right);
        }

        /// <inheritdoc />
        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        #endregion
    }
}
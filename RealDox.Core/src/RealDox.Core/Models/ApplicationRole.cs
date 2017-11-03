using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Identity;
using RealDox.Core.Interfaces;

namespace RealDox.Core.Models
{
    public class ApplicationRole : IdentityRole, IEntity
    {
        #region Properties
        public string Description { get; set; }

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
        
        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<string>> Users { get; } = new List<IdentityUserRole<string>>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityRoleClaim<string>> Claims { get; } = new List<IdentityRoleClaim<string>>();
        

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
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                var value = info.GetValue(this, null) ?? "(null)";
                sb.AppendLine(info.Name + ": " + value.ToString());
            }

            return sb.ToString();
        }

        #endregion

        #region Field Members
        private PropertyInfo[] _PropertyInfos = null;

        #endregion
       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using RealDox.Core.Interfaces;

namespace RealDox.Core.Models
{
    /// <summary>
    /// Identity implementation of IUser.
    /// </summary>
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser, IEntity
    {
        #region Properties
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public string Title { get; set; }

        //[Required]
        [MaxLength(120)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        //[Required]
        [MaxLength(30)]
        [Display(Name = "City")]
        public string City { get; set; }

        //[Required]
        [MaxLength(20)]
        [Display(Name = "State")]
        public string State { get; set; }

        //[Required]
        [MaxLength(10)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        //[Required]
        [Display(Name = "Fax")]
        [MaxLength(13)]
        public string Fax { get; set; }

        [Display(Name = "Sec Phone")]
        [MaxLength(13)]
        public string SecPhone { get; set; }

        public string ImageProfile { get; set; }

        public bool? Active { get; set; }

        [ScaffoldColumn(false)]
        public virtual DateTime? LastLoginTime { get; set; }
        
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
        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; } = new List<IdentityUserLogin<string>>();


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

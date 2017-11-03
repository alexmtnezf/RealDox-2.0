using System.ComponentModel.DataAnnotations;

namespace RealDox.Core.Models
{
    public class TodoCategory : Entity
    {
        /// <summary>
        /// Indicates whether the category is active or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Category name.
        /// </summary>
        /*[StringLength(Config.TodoCategoryDescriptionMaxLength)]*/
        public string Name { get; set; }

        /// <summary>
        /// Category description.
        /// </summary>
        /*[StringLength(Config.TodoCategoryDescriptionMaxLength)]*/
        public string Description { get; set; }

        /// <summary>
        /// The color string for this category. Can be any value that assignable in CSS.
        /// </summary>
       /* [StringLength(Config.TodoCategoryColorMaxLength)]-*/
        public string Color { get; set; }

        /// <summary>
        /// The icon class string for this category. Can be any value that assignable in HTML class attribute.
        /// </summary>
        /*[StringLength(Config.TodoCategoryIconMaxLength)]*/
        public string IconClass { get; set; }
        
    }
}
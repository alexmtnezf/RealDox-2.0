using System.ComponentModel.DataAnnotations;
using RealDox.Core.Attributes;

namespace RealDox.Core.Models
{
    [Help("https://docs.microsoft.com/dotnet/csharp/tour-of-csharp/attributes")]
    public class TodoItem : Entity
    {
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string Notes { get; set; }

        public bool Done { get; set; }
        [Help("https://docs.microsoft.com/dotnet/csharp/tour-of-csharp/attributes",
    Topic = "Description")]
        public string Description { get; set; }
    }
}
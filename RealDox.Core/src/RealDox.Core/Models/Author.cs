﻿using System.ComponentModel.DataAnnotations;

namespace RealDox.Core.Models
{
    public class Author
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }
        [MaxLength(30)]
        public string TwitterAlias { get; set; }
    }
}

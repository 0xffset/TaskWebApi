using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Taks.Core.Entities.General
{
    public class Role : IdentityRole<int>
    {

        [Required]
        [MaxLength(10)]
        public required string Code { get; set; }
        public bool IsActive { get; set; }
        public int? EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

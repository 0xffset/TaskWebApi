using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taks.Core.Entities.General
{
    public class User : IdentityUser<int>
    {
      
        [Required, StringLength(maximumLength: 100, MinimumLength = 2)]
        public required string FullName { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public int RoleId { get; set; }
        public int? EntryBy { get; set; }
        public DateTime? EntryDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
    }
}

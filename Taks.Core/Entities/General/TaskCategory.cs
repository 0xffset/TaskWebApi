using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taks.Core.Entities.General
{
    [Table("Categories")]

    public class TaskCategory : Base<int>
    {
        [Key]
        public new int Id { get; set; }
        [Required, StringLength(maximumLength: 50, MinimumLength = 2)]
        public required string Name { get; set; }
        public bool Status { get; set; }
    }
}

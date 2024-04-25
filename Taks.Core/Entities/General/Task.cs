using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taks.Core.Entities.General
{
    [Table("Tasks")]
    public class Task : Base<int>
    {
        [Key]
        public new int Id { get; set; }

        [Required, StringLength(maximumLength: 50, MinimumLength = 2)]
        public required string Title { get; set; }

        [Required, StringLength(maximumLength: 1000, MinimumLength = 10)]
        public required string Description { get; set; }

        [Required]
        public DateTime DueTime { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public bool Status { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public  TaskCategory Category { get; set; }

    }
}

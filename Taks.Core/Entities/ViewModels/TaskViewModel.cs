using System.ComponentModel.DataAnnotations;

namespace Taks.Core.Entities.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public  string? Title { get; set; }
        public  string? Description { get; set; }
        public bool Status { get; set; }
        public DateTime DueTime { get; set; }
        public int CategoryId { get; set; }

    }

    public class TaskCreateViewModel
    {
        [Required, StringLength(maximumLength: 50, MinimumLength = 2)]

        public required string Title { get; set; }
        [Required, StringLength(maximumLength: 500, MinimumLength = 2)]

        public required string Description { get; set; }
        [Required]

        public bool Status { get; set; }
        [Required]

        public DateTime DueTime { get; set; }

        public int CategoryId { get; set; }

    }

    public class TaskUpdateViewModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool Status { get; set; }
        public DateTime DueTime { get; set; }

        public int CategoryId { get; set; }
    }
}

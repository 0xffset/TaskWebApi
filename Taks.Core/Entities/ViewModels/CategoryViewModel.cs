using System.ComponentModel.DataAnnotations;

namespace Taks.Core.Entities.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Status { get; set; }

        public static implicit operator CategoryViewModel(TaskViewModel v)
        {
            throw new NotImplementedException();
        }
    }

    public class CategoryCreateViewModel
    {
        [Required, StringLength(maximumLength: 50, MinimumLength = 2)]
        public string? Name { get; set; }
    }

    public class CategoryUpdateViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(maximumLength: 50, MinimumLength = 2)]
        public string? Name { get; set; }

        public bool Status { get; set; }


    }
}

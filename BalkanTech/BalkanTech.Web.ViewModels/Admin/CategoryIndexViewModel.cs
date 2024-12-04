using System.ComponentModel.DataAnnotations;
using static BalkanTech.Common.Constants;

namespace BalkanTech.Web.ViewModels.Admin
{
    public class CategoryIndexViewModel
    {

        public Guid Id { get; set; }
        [Required]
        [MinLength(MinValueCategoryNameLength)]
        [MaxLength(MaxValueCategoryNameLength)]
        public string Name { get; set; } = string.Empty;
    }
}

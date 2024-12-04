using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Web.ViewModels.Admin
{
    public class ManageCategoriesViewModel
    {
        public required string Title { get; set; } 
        public required string AddCategoryAction { get; set; } 
        public required string DeleteCategoryAction { get; set; }
        public IEnumerable<CategoryIndexViewModel> Categories { get; set; } = new List<CategoryIndexViewModel>();
    }
}

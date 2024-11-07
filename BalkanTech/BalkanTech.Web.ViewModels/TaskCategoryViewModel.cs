using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Web.ViewModels
{
    public class TaskCategoryViewModel
    {
        public Guid Id { get; set; }
        public string TaskCategoryName { get; set; } = string.Empty;
    }
}

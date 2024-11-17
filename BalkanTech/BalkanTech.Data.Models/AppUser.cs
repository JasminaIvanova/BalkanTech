
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static BalkanTech.Common.Constants;

namespace BalkanTech.Data.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public AppUser()
        {
            this.Id = Guid.NewGuid();
        }
        [Required]
        [MinLength(MinValueNameUserLength)]
        [MaxLength(MaxValueNameUserLength)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MinLength(MinValueNameUserLength)]
        [MaxLength(MaxValueNameUserLength)]
        public string LastName { get; set; } = string.Empty ;
        public ICollection<AssignedTechnicianTask> AssignedTechniciansTasks { get; set; } = new List<AssignedTechnicianTask>();

    }
}

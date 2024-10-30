
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Data.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public AppUser()
        {
            this.Id = Guid.NewGuid();
        }
        public string FirstName { get; set; }
    }
}

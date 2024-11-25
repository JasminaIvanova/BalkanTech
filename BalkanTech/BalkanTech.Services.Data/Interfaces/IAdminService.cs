using BalkanTech.Web.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Services.Data.Interfaces
{
    public interface IAdminService
    {
        Task<List<TechsIndexViewModel>> IndexGetAllTechsAsync();
        Task ChangeRoleOfUserAsync(Guid userId, string role);
    }
}

using Authentication.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Repository.Repository.IRepository
{
    public interface IUnitOfWorkRepository
    {
        UserManager<Users> UserManager { get; }
        RoleManager<Roles> RoleManager { get; }
        Task Save(string? userId);
    }
}

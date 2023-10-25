using Authentication.Data;
using Authentication.Models.DBModels;
using Authentication.Models.Entities;
using Authentication.Repository.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Repository.Repository
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        public ApplicationDbContext _context;
        public UserManager<Users> UserManager { get; private set; }
        public RoleManager<Roles> RoleManager { get; private set; }

        public UnitOfWorkRepository(ApplicationDbContext context, UserManager<Users> userManager = null, RoleManager<Roles> roleManager = null)
        {
            _context = context;
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public async Task Save(string? userId)
        {
            var entries = _context.ChangeTracker.Entries().Where(e => e.Entity is BaseModel && e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseModel)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                    ((BaseModel)entityEntry.Entity).CreatedBy = userId;
                }
                else
                {
                    if (((BaseModel)entityEntry.Entity).IsDeleted)
                    {
                        ((BaseModel)entityEntry.Entity).DeletedAt = DateTime.UtcNow;
                        ((BaseModel)entityEntry.Entity).DeletedBy = userId;
                    }
                    else
                    {
                        ((BaseModel)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
                        ((BaseModel)entityEntry.Entity).UpdatedBy = userId;
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartDormitory.Data.Models;

namespace SmartDormitory.App.Data
{
    public class SmartDormitoryContext : IdentityDbContext<User>
    {
        public SmartDormitoryContext()
        {

        }
        public SmartDormitoryContext(DbContextOptions<SmartDormitoryContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

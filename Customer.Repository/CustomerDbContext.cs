

using System;
using System.Linq;
using Customer.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Customer.Repository
{
    public class CustomerDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerDbContext(DbContextOptions<CustomerDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Domain.Customer> Customer { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is Entity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((Entity)entityEntry.Entity).CreatedDate = DateTime.Now;
                    ((Entity)entityEntry.Entity).CreatedBy = _httpContextAccessor.HttpContext.Request.Headers["audity-user"].ToString();
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    ((Entity)entityEntry.Entity).ModifiedDate = DateTime.Now;
                    ((Entity)entityEntry.Entity).ModifiedBy = _httpContextAccessor.HttpContext.Request.Headers["audity-user"].ToString();
                }
            }

            return base.SaveChanges();
        }
    }
}

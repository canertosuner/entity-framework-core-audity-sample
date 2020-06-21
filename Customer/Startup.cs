using Customer.Domain;
using Customer.Repository;
using Customer.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDatabase(services);
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerService, CustomerService>();

            services.AddTransient<ICustomerAssembler, CustomerAssembler>();
            services.AddHttpContextAccessor();
            services.AddTransient<ICustomerService, CustomerService>();

            services.AddControllers().AddApplicationPart(typeof(Startup).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("DefaultApi", "api/{controller}/{id}");
            });
        }

        public virtual void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<CustomerDbContext>(options =>
                options.UseInMemoryDatabase(Configuration.GetSection("CustomerDbConnString").Value));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetmysql.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Swashbuckle.AspNetCore.Swagger;

namespace usersrestapi {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            // other service configurations go here
            services.AddDbContext<testingContext> ( // replace "YourDbContext" with the class name of your DbContext
                options => options.UseMySql ("Server=localhost;Database=testing;User=root;Password=testingnat9;", // replace with your Connection String
                    mysqlOptions => {
                        mysqlOptions.ServerVersion (new Version (8, 0, 12), ServerType.MySql); // replace with your Server Version and Type
                    }
                ));

            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen (c => {
                c.SwaggerDoc ("v1", new Info { Title = "Users API", Version = "v1" });
            });
            services.AddMvcCore ().AddApiExplorer ();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseHsts ();
            }

            app.UseHttpsRedirection ();
            app.UseMvc ();

            app.UseSwagger (c => { });
            app.UseSwaggerUI (c => {
                c.SwaggerEndpoint ("/swagger/v1/swagger.json", "Users API");
            });
        }
    }
}
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.HttpOverrides;
using SpentBulletsAPI.Models;
namespace SpentBulletsAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        readonly string CorsLocalPolicy = "_CorsLocalPolicy";

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.  
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.  
            services.AddCors(options =>
            {
                options.AddPolicy(CorsLocalPolicy, builder =>
                {
                    builder.WithOrigins("http://localhost:3311",
                        "http://localhost:5000",
                        "http://localhost:3000",
                        "http://spent-bullets.firechildren.net")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            //services.Add(new ServiceDescriptor(typeof(Context), new Context(Configuration.GetConnectionString("DefaultConnection"))));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.  
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseCors(CorsLocalPolicy);

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
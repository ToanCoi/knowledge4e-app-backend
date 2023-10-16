using Knowledge4e.Core.Extensions;
using Knowledge4e.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;

namespace Knowledge4e.Web
{
    public class Startup
    {
        private readonly string ApiName = "Knowlegde4eApi";
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDirectoryBrowser();

            var configuration = new ConfigurationBuilder()
           .AddJsonFile($"appsettings.{Environment.EnvironmentName}.json")
           .Build();

            IdentityModelEventSource.ShowPII = true;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = ApiName, Version = "v1" });
            });
            services.AddSingleton(configuration);

            TinyMapperExtension.Bind();

            StartupExtensions.AddCors(services);
            services.AddMvc(x => x.EnableEndpointRouting = false);
            services.AddHttpContextAccessor();
            services.InjectDependencies();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ICorsService corsService, Microsoft.AspNetCore.Cors.Infrastructure.ICorsPolicyProvider corsPolicyProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseMiddlewares();
            app.UseRouting();
            app.UseStaticFiles();
            app
               .UseCors(policy =>
                   policy
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials()
                       .WithOrigins("http://localhost:3000"));

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", ApiName);
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

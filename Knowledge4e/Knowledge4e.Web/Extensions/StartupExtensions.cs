using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Knowledge4e.ApplicationCore.Interfaces;
using Knowledge4e.ApplicationCore.MiddleWare;

namespace Knowledge4e.ApplicationCore.Extensions
{
    public static class StartupExtensions 
    {
        public static void UseMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlingMiddleWare>();
        }

        public static void AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowCROSPolicy",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                    });
            });
        }

        public static void InjectDependencies(this IServiceCollection services)
        {
            //base
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
        }
    }
}

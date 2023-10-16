using Knowledge4e.Core.Services;
using Knowledge4e.Core.Services.BaseService;
using Knowledge4e.Infarstructure.Repositories;
using Knowledge4e.Infarstructure.Repositories.BaseRepository;
using Knowledge4e.Web.Extensions;
using Knowledge4e.Web.MiddleWare;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Knowledge4e.Web.Extensions
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
            //account
            services.AddScoped(typeof(IAccountService), typeof(AccountService));
            services.AddScoped(typeof(IAccountRepository), typeof(AccountRepository));
        }
    }
}

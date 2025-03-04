
using ExtModule.API.App.Interfaces;
using ExtModule.API.Application.Factory;
using ExtModule.API.Application.Interfaces;
using ExtModule.API.Infra.Repositories;
using ExtModule.API.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtModule.API.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterServices(this IServiceCollection services)
        {

            services.AddTransient<IERPRepository,RepositoryERP>();
            services.AddTransient<ICRMRepository, RepositoryCRM>();
            services.AddScoped<IJWTTokenService, JwtTokenService>();

            services.AddTransient<IRepositoryFactory, RepositoryFactory>();
        }
    }
}

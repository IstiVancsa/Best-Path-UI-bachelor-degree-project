using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazoredSessionStorage(this IServiceCollection services)
        {
            return services.AddScoped<ISessionStorageDataService, SessionStorageDataService>();
        }
    }
}

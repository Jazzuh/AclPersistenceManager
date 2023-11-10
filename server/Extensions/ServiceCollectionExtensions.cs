using System;
using AclManager.Server.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace AclManager.Server.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseAdapter(this IServiceCollection services, string adapterType)
        {
            switch (adapterType)
            {
                case "kvp":
                    services.AddTransient<IDatabaseAdapter, KvpAdapter>();
                    break;
                case "mysql":
                    services.AddTransient<IDatabaseAdapter, MySqlAdapter>();
                    break;
                default:
                    throw new ArgumentException($"Invalid database adapter provided: {adapterType}");
            }

            return services;
        }
    }
}

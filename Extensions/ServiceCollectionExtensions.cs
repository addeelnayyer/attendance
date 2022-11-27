using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Aquila360.Attendance.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAllTypesAsScopedServicesInNamespaceAs<T>(this IServiceCollection services, List<Type> exclusions = null)
        {
            foreach (var type in typeof(T).GetAllTypesInNamespace())
            {
                if (exclusions != null && exclusions.Contains(type))
                {
                    continue;
                }

                var nonGenericInterface = type.GetInterfaces().FirstOrDefault(i => !i.IsGenericType && i.Name == $"I{type.Name}");
                if (nonGenericInterface != null)
                {
                    services.AddScoped(nonGenericInterface, type);
                    continue;
                }

                nonGenericInterface = type.GetInterfaces().FirstOrDefault(i => !i.IsGenericType);
                if (nonGenericInterface != null)
                {
                    services.AddScoped(nonGenericInterface, type);
                    continue;
                }

                var genericInterface = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType);
                if (genericInterface != null)
                {
                    services.AddScoped(genericInterface, type);
                }
            }

            return services;
        }
    }
}

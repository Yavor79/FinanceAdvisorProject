using FinanceAdvisor.Domain.IRepositories;
//using FinanceAdvisor.Infrastructure.Identity;
using FinanceAdvisor.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FinanceAdvisor.Infrastructure.Extensions
{
    public static class ServicesExtensions
    {
        private static readonly string ProjectInterfacePrefix = "I";
        private static readonly string ServiceTypeSuffix = "Service";

        private static readonly string RepositoryTypeSuffix = "Repository";
        public static IServiceCollection RegisterRepositories(this IServiceCollection services, Assembly modelsAssembly)
        {
            Type[] repoClasses = modelsAssembly.GetTypes()
                .Where(t => t.Name.EndsWith(RepositoryTypeSuffix)
                && !t.IsInterface 
                && !t.IsAbstract)
                .ToArray();

            foreach (Type repoClass in repoClasses)
            {
                Type? repoInterface = repoClass
                    .GetInterfaces()
                    .FirstOrDefault(i => i.Name == $"{ProjectInterfacePrefix}{repoClass.Name}");

                if (repoInterface == null)
                {
                    throw new ArgumentException(string.Format($"InterfaceNotFound for {repoClass.Name}"));
                }

                services.AddScoped(repoInterface, repoClass);
            }

            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services, Assembly assembly)
        {
            Type[] serviceClasses = assembly
                .GetTypes()
                .Where(t => t.Name.EndsWith(ServiceTypeSuffix) 
                    && !t.IsInterface)
                .ToArray();

            foreach (Type serviceClass in serviceClasses)
            {
                Type? serviceInterface = serviceClass
                    .GetInterfaces()
                    .FirstOrDefault(i => i.Name == $"{ProjectInterfacePrefix}{serviceClass.Name}");

                if (serviceInterface == null)
                {
                    throw new ArgumentException($"InterfaceNotFound for {serviceClass.Name}");
                }

                services.AddScoped(serviceInterface, serviceClass);
            }
            return services;
        }
    }
}

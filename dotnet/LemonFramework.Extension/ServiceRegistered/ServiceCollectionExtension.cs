using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LemonFramework.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace LemonFramework.Extension.ServiceRegistered
{
    /// <summary>
    /// 废弃  2025.03.05
    /// </summary>
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection ServiceRegister(this IServiceCollection services)
        {
            var asmCore = Assembly.Load("LemonFramework.Service");
            // 单例生命周期
            var singletonTypes = asmCore.GetTypes().Where(m => m.IsAssignableFrom(typeof(ISingleton)) && !m.IsAbstract && !m.IsInterface);
            foreach (Type? singletoType in singletonTypes)
            {
                Type? interfaceType = singletoType.GetInterfaces().Where(m => m!=typeof(ISingleton)).FirstOrDefault();
                if (interfaceType != null)
                {
                    services.AddSingleton(interfaceType, singletoType);
                }
            }
            // 瞬态生命周期
            var transientTypes = asmCore.GetTypes().Where(m => m.IsAssignableFrom(typeof(ITransient)) && !m.IsAbstract && !m.IsInterface);
            foreach (Type? transientType in transientTypes)
            {
                Type? interfaceType =  transientType.GetInterfaces().Where(m => m!=typeof(ITransient)).FirstOrDefault();
                if (interfaceType != null)
                {
                    services.AddTransient(interfaceType, transientType);
                }
            }
            // 作用域生命周期
            var scopedTypes = asmCore.GetTypes().Where(m => m.IsAssignableFrom(typeof(IScoped)) && !m.IsAbstract && !m.IsInterface);
            foreach (Type? scopedType in scopedTypes)
            {
                Type? interfaceType = scopedType.GetInterfaces().Where(m => m!=typeof(IScoped)).FirstOrDefault();
                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, scopedType);
                }
            }
            return services;
        }
    }
}
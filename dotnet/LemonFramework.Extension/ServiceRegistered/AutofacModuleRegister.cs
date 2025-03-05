using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.DynamicProxy;
using LemonFramework.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel;
using Microsoft.VisualBasic;

namespace LemonFramework.Extension.ServiceRegistered
{
    public class AutofacModuleRegister : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            List<Assembly> assemblies = new();
            var libs = DependencyContext.Default.CompileLibraries.Where(x => !x.Serviceable && x.Type == "project").ToList();
            foreach (var lib in libs)
            {
                assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name)));
            }

            builder.RegisterAssemblyTypes(assemblies.ToArray()).Where(o => o.IsAssignableTo<ControllerBase>() && !o.IsAbstract && !o.IsInterface).AsSelf().PropertiesAutowired();

            // builder.RegisterAssemblyTypes(assemblies.ToArray()).Where(o => o.IsAssignableTo<ITransient>() && !o.IsAbstract && !o.IsInterface).AsSelf().AsImplementedInterfaces().PropertiesAutowired().InterceptedBy();
            builder.RegisterAssemblyTypes(assemblies.ToArray()).Where(o => o.IsAssignableTo<ISingleton>() && !o.IsAbstract && !o.IsInterface).AsSelf().AsImplementedInterfaces().SingleInstance().PropertiesAutowired(); // 单例服务注册
            builder.RegisterAssemblyTypes(assemblies.ToArray()).Where(o => o.IsAssignableTo<ITransient>() && !o.IsAbstract && !o.IsInterface).AsSelf().AsImplementedInterfaces().PropertiesAutowired(); // 瞬态服务注册
            builder.RegisterAssemblyTypes(assemblies.ToArray()).Where(o => o.IsAssignableTo<IScoped>() && !o.IsAbstract && !o.IsInterface).AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope().PropertiesAutowired(); // 作用域服务注册
        }
    }
}
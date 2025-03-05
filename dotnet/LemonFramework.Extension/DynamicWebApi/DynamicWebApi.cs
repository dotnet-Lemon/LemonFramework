using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LemonFramework.Extension.DynamicWebApi
{
    public static class DynamicWebApi
    {
        /// <summary>
        /// 为 IMvcBuilder 添加动态 WebAPI 功能
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddDynamicWebApi(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureApplicationPartManager(app => {
                app.FeatureProviders.Add(new ApplicationServiceControllerFeatureProvider());
            });

            builder.Services.Configure<MvcOptions>(options => {
                options.Conventions.Add(new ApplicationServiceConvertion());
            });

            return builder;
        }

        /// <summary>
        /// 为 IMvcCoreBuilder 添加动态 WebAPI 功能。
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMvcCoreBuilder AddDynamicWebApi(this IMvcCoreBuilder builder)
        {
                        if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureApplicationPartManager(app => {
                app.FeatureProviders.Add(new ApplicationServiceControllerFeatureProvider());
            });

            builder.Services.Configure<MvcOptions>(options => {
                options.Conventions.Add(new ApplicationServiceConvertion());
            });

            return builder;
        }
    }
}
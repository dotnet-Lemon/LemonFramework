using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LemonFramework.Domain.Common;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace LemonFramework.Extension.DynamicWebApi
{
    public class ApplicationServiceControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            if (typeof(IDynamicWebApi).IsAssignableFrom(typeInfo))
            {
                var type = typeInfo.AsType();
                if (typeof(IDynamicWebApi).IsAssignableFrom(type) || typeInfo.IsDefined(typeof(DynamicWebApiAttribute), true))
                {
                    if (typeInfo.IsPublic && !typeInfo.IsAbstract && !typeInfo.IsGenericType && !typeInfo.IsInterface)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
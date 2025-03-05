using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LemonFramework.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Routing;

namespace LemonFramework.Extension.DynamicWebApi
{
    public class ApplicationServiceConvertion : IApplicationModelConvention
    {
        /// <summary>
        /// 应用约定
        /// </summary>
        /// <param name="application"></param>
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var controllerType = controller.ControllerType.AsType();

                if (typeof(IDynamicWebApi).IsAssignableFrom(controllerType))
                {
                    foreach (var item in controller.Actions)
                    {
                        ConfigureSelector(controller.ControllerName, item);
                    }
                }
                ConfigureParameters(controller);
            }
        }

        /// <summary>
        /// 配置选择器
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="action"></param>
        private void ConfigureSelector(string controllerName, ActionModel action)
        {
            // 如果属性路由模板为空，则移除
            for (var i = 0; i < action.Selectors.Count; i++)
            {
                if (action.Selectors[i].AttributeRouteModel is null)
                {
                    action.Selectors.Remove(action.Selectors[i]);
                }
            }
            // 去除路径中的 AppService 后缀
            if (controllerName.EndsWith("AppService"))
            {
                controllerName = controllerName.Substring(0, controllerName.Length - 10);
            }
            // 如果有选择器，则遍历选择器，添加默认路由
            if (action.Selectors.Any())
            {
                foreach (var item in action.Selectors)
                {
                    var routePath = string.Concat("api/", controllerName + "/", action.ActionName).Replace("//", "/");
                    var routeModel = new AttributeRouteModel(new RouteAttribute(routePath));
                    // 如果没有设置路由，则添加路由
                    if (item.AttributeRouteModel == null)
                    {
                        item.AttributeRouteModel = routeModel;
                    }
                }
            }
            else
            {
                action.Selectors.Add(CreateActionSelector(controllerName, action));
            }
        }

        /// <summary>
        /// 创建 Action 选择器
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private SelectorModel CreateActionSelector(string controllerName, ActionModel action)
        {
            SelectorModel selectorModel = new();
            string? httpMethod = string.Empty;
            var routeAttribute = action.ActionMethod.GetCustomAttributes(typeof(HttpMethodAttribute), false);
            if (routeAttribute != null && routeAttribute.Any())
            {
                httpMethod = routeAttribute.SelectMany(m => (m as HttpMethodAttribute).HttpMethods).ToList().Distinct().FirstOrDefault();
            }
            else
            {
                var actionName = action.ActionMethod.Name.ToUpper();

                if (actionName.StartsWith("GET") || actionName.StartsWith("QUERY"))
                {
                    httpMethod = "Get";
                }
                if (actionName.StartsWith("CREATE") || actionName.StartsWith("SAVE") || actionName.StartsWith("INSERT") || actionName.StartsWith("ADD"))
                {
                    httpMethod = "Post";
                }
                if (actionName.StartsWith("UPDATA") || actionName.StartsWith("EDIT"))
                {
                    httpMethod = "Put";
                }
                if (actionName.StartsWith("DELETE") || actionName.StartsWith("REMOVE"))
                {
                    httpMethod = "Delete";
                }

                if (httpMethod == string.Empty)
                {
                    httpMethod = "Post";
                }
            }
            return ConfigureSelectModel(selectorModel, action, controllerName, httpMethod);
        }

        /// <summary>
        /// 配置选择器类型
        /// </summary>
        /// <param name="selectorModel"></param>
        /// <param name="actionModel"></param>
        /// <param name="controllerName"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        private SelectorModel ConfigureSelectModel(SelectorModel selectorModel, ActionModel actionModel, string controllerName, string? httpMethod)
        {
            // 路由地址拼接
            var routePath = string.Concat("api/", controllerName + "/", actionModel.ActionName).Replace("//", "/");
            // 添加路由选择器
            selectorModel.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(routePath));
            // 添加 HttpMethod
            selectorModel.ActionConstraints.Add(new HttpMethodActionConstraint(new[] {httpMethod}));
            return selectorModel;
        }

        /// <summary>
        /// 配置请求模板
        /// </summary>
        /// <param name="controllerModel"></param>
        private void ConfigureParameters(ControllerModel controllerModel)
        {
            foreach (var action in controllerModel.Actions)
            {
                foreach (var parameter in action.Parameters)
                {
                    if (parameter.BindingInfo != null)
                        continue;

                    if (parameter.ParameterType.IsClass && parameter.ParameterType != typeof(string) && parameter.ParameterType != typeof(IFormFile))
                    {
                        var httpMethod = action.Selectors.SelectMany(temp => temp.ActionConstraints).OfType<HttpMethodActionConstraint>().SelectMany(t => t.HttpMethods).ToList();
                        if (httpMethod.Contains("GET") || httpMethod.Contains("DELETE") || httpMethod.Contains("TRACE") || httpMethod.Contains("HEAD"))
                        {
                            continue;
                        }

                        parameter.BindingInfo = BindingInfo.GetBindingInfo(new [] { new FromBodyAttribute()});
                    }
                }
            }
        }
    }
}
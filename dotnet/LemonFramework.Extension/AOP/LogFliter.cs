using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace LemonFramework.Extension.AOP
{
    public class LogFliter : IInterceptor
    {
        // private readonly IHubContext<ChatHub> _hubContext;
        private readonly IHttpContextAccessor _accessor;

        public LogFliter(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        
        public void Intercept(IInvocation invocation)
        {
            DateTime startTime = DateTime.Now;
            string dataIntercept;

            try 
            {
                dataIntercept = $"{ DateTime.Now.ToString("yyyyMMddHHmmss") }" + $"当前执行方法：{ invocation.Method.Name }" + $"参数是：{ string.Join(",", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray())} \r\n";

                invocation.Proceed();

                // 异步获取异常
                 if (IsAsyncMethod(invocation.Method))
                 {
                    if (invocation.Method.ReturnType == typeof(Task))
                    {
                        invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithPostActionAndFinally((Task) invocation.ReturnValue, 
                        async () => await LogSuccess(invocation, startTime), 
                        ex => {
                            LogEx(ex);
                        });
                    }
                    else 
                    {
                        invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(invocation.Method.ReturnType.GenericTypeArguments[0], 
                        invocation.ReturnValue, 
                        async (o) => await LogSuccess(invocation, startTime, o),
                        ex => {
                            LogEx(ex);
                        });
                        
                    }
                 }
                 else //同步
                 {
                    string jsonResult;
                    DateTime endTime = DateTime.Now;
                    
                    try
                    {
                        jsonResult = JsonConvert.SerializeObject(invocation.ReturnValue);
                    }
                    catch (Exception ex)
                    {
                        jsonResult = "无法序列化，可能是兰姆达表达式等原因造成，按照框架优化代码\r\n" + ex.ToString();
                    }
                    string log = $"响应时间：{(endTime - startTime).Milliseconds.ToString()} ms \r\n执行结果：{jsonResult}";

                    var path = Directory.GetCurrentDirectory() + @"\Log";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory (path);
                    }
                    string fileName = path + $@"\InterceptLog-{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
                    StreamWriter sw = File.AppendText(fileName);
                    sw.WriteLine(log);
                    sw.Close();
                 }
            }
            catch (Exception ex)
            {
                LogEx(ex);
                throw;
                // dataIntercept = ($"方法执行中出现异常：{ ex.Message }");
            }

           
            // dataIntercept += ($"被拦截方法执行完毕，返回结果：{ invocation.ReturnValue }");
            // var path = Directory.GetCurrentDirectory() + @"\Log";
            // if (!Directory.Exists(path))
            // {
            //     Directory.CreateDirectory(path);
            // }

            // string fileName = path + $@"\InterceptLog-{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
            // StreamWriter sw = File.AppendText(fileName);
            // sw.WriteLine(dataIntercept);
            // sw.Close();
        }

        private bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) || (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            );
        }

        /// <summary>
        /// 成功日志记录
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="startTime"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        private async Task LogSuccess (IInvocation invocation, DateTime startTime, object? o = null)
        {   
            string? log;
            string jsonResult;
            DateTime endTime = DateTime.Now;
            try
            {
                jsonResult = JsonConvert.SerializeObject(invocation.ReturnValue);
            }
            catch (Exception ex)
            {
                jsonResult = "无法序列化，可能是兰姆达表达式等原因造成，按照框架优化代码\r\n" + ex.ToString();
            }

            if (o == null)
            {
                log = $"响应时间：{(endTime - startTime).Milliseconds.ToString()} ms \r\n执行结果：{jsonResult}";
            }
            else
            {
                log = $"响应时间：{(endTime - startTime).Milliseconds.ToString()} ms \r\n执行结果：{jsonResult} \r\n响应参数：{JsonConvert.SerializeObject(o)}";
            }

            await Task.Run(() => 
            {
                var path = Directory.GetCurrentDirectory() + @"\Log";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory (path);
                }

                string fileName = path + $@"\InterceptLog-{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
                StreamWriter sw = File.AppendText(fileName);
                sw.WriteLine(log);
                sw.Close();
            });    
        }

        /// <summary>
        /// 异常日志记录
        /// </summary>
        /// <param name="ex"></param>
        private void LogEx(Exception ex)
        {
            if (ex != null)
            {
                string log = "InnerException-内部结果：\r\n" + (ex.InnerException == null ? "" : ex.InnerException.InnerException.ToString()) + "\r\n StackTrace-堆栈跟踪：\r\n" + (ex.StackTrace == null ? "" : ex.StackTrace.ToString());

                var path = Directory.GetCurrentDirectory() + @"\LogEx";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory (path);
                }
                string fileName = path + $@"\ExceptionLog-{DateTime.Now.ToString("yyyyMMddHHmmss")}.log";
                StreamWriter sw = File.AppendText(fileName);
                sw.WriteLine(log);
                sw.Close();
            }
        }
    }

    internal static class InternalAsyncHelper
    {
        public static async Task AwaitTaskWithPostActionAndFinally(Task actualReturnValue, Func<Task> postAction, Action<Exception> finalAction)
        {
            Exception? exception = null;

            try 
            {
                await actualReturnValue;
                await postAction();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                finalAction(exception);
            }
        }

        public static async Task<T> AwaitTaskWithPostActionAndFinallyAndGetResult<T>(Task<T> actualReturnValue, Func<object, Task> postAction, Action<Exception> finalAction)
        {
            Exception? exception = null;
            try
            {
                var result = await actualReturnValue;
                await postAction(result);
                return result;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                finalAction(exception);
            }
        }

        public static object CallAwaitTaskWithPostActionAndFinallyAndGetResult(Type taskReturnType, object actualReturnValue, Func<object, Task> action, Action<Exception> finalAction)
        {
            return typeof(InternalAsyncHelper).GetMethod("AwaitTaskWithPostActionAndFinallyAndGetResult", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(taskReturnType).Invoke(null, new object[] { actualReturnValue, action, finalAction });
        }
    }
}
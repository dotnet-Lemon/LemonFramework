using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace LemonFramework.Common.Helper
{
    public class Appsettings
    {
        public static IConfiguration Configuration { get; set; }

        public string contentPath { get; set; }

        public Appsettings(string contentPath)
        {
            string Path = "appsetting.json";
            Configuration = new ConfigurationBuilder()
                .SetBasePath(contentPath)
                .Add(new JsonConfigurationSource
                {
                    Path = Path,
                    Optional = false,
                    ReloadOnChange = true
                }).Build();
        }

        public Appsettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections">节点配置</param>
        /// <returns></returns>
        public static string app(params string[] sections)
        {
            try
            {
                if (sections.Any())
                {
                    return Configuration[string.Join(":", sections)];
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        /// <summary>
        /// 递归获取配置信息数组
        /// </summary>
        /// <param name="sections"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> app<T>(params string[] sections)
        {
            List<T> list = new();
            Configuration.Bind(string.Join(":", sections), list);
            return list;
        }

        /// <summary>
        /// 根据路径  configuration["App:Name"];
        /// </summary>
        /// <param name="sectionsPath"></param>
        /// <returns></returns>
        public static string GetValue(string sectionsPath)
        {
            try 
            {
                return Configuration[sectionsPath];
            }
            catch (Exception ex)
            {

            }

            return "";
        }
    }
}
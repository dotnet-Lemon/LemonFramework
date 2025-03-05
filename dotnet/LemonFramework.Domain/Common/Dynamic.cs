using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LemonFramework.Domain.Common
{
    public interface IDynamicAutoMapper;

    public interface IDynamicWebApi;

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DynamicWebApiAttribute : Attribute
    {

    }
}
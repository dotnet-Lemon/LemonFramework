using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LemonFramework.Common.Helper
{
    public static class UtilConvert
    {
        public static bool ObjToBool(this object obj)
        {
            bool  reval = false;
            if (obj != null && obj != DBNull.Value && bool.TryParse(obj.ToString(), out reval))
            {
                return reval;
            }
            return false;
        }
    }
}
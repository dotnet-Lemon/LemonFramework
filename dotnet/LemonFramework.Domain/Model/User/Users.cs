using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LemonFramework.Domain.Model.User
{
    public class Users : BaseEntity
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        /// <returns></returns>
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 用户名
        /// </summary>
        /// <value></value>
        public string UserNaame { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        /// <value></value>
        public string RealName { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        /// <value></value>
        public string Phone { get; set; }
    }
}
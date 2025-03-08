using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LemonFramework.Domain.Model
{
    public class BaseEntity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        /// <returns></returns>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        /// <value></value>
        [Column(TypeName = "dateTime")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改时间
        /// </summary>
        /// <value></value>
        [Column(TypeName = "dateTime")]
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        /// <value></value>
        public bool? IsEnabled { get; set; } = true;

        /// <summary>
        /// 是否删除
        /// </summary>
        /// <value></value>
        public bool? IsDelete { get; set; } = false;
    }
}
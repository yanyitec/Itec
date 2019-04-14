using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Entities
{
    public class RecordEntity<T>:Entity<T>
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建者Id
        /// </summary>
        public T CreatorId { get; set; }
        /// <summary>
        /// 创建者名称
        /// </summary>
        public string CreatorName { get; set; }
        /// <summary>
        /// 创建者JSON
        /// </summary>
        public string CreatorJSON { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime ModifyTime { get; set; }
        /// <summary>
        /// 更新者Id
        /// </summary>
        public T ModifierId { get; set; }

        /// <summary>
        /// 更新者Id
        /// </summary>
        public string ModifierName { get; set; }

        /// <summary>
        /// 更新者JSON
        /// </summary>
        public string ModifierJSON { get; set; }

        /// <summary>
        /// 软删除标记
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 最后变更动作
        /// </summary>
        public string LastOperation { get; set; }

        public void DoCreate(IUser<T> creator,string operation="Create") {
            this.CreateTime = this.ModifyTime = DateTime.Now;
            this.CreatorId = this.ModifierId = creator.Id;
            this.CreatorName = this.ModifierName = creator.Name;
            this.LastOperation = operation;
        }

        public void DoModify(IUser<T> Modifier, string operation = "Modify")
        {
            this.ModifyTime = DateTime.Now;
            this.ModifierId = Modifier.Id;
            this.ModifierName = Modifier.Name;
            this.LastOperation = operation;
        }
    }
}

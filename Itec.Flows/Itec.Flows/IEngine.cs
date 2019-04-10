using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Flows
{
    public interface IEngine
    {
        /// <summary>
        /// 新创建一个工作流实例
        /// </summary>
        /// <param name="diagramId">流程图名称</param>
        /// <param name="creator">创建者</param>
        /// <param name="inputs">输入</param>
        /// <returns></returns>
        Task<ActivityState> CreateFlowAsync(string diagramId,IUser creator,object inputs);
        Task<ActivityState> LockActivityAsync(Guid activityId, IUser dealer);
    }
}

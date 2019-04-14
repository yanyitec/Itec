using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Datas
{
    public interface IPersistentData:IWritableData, IPersistent
    {
        new IPersistentData Load();
        new Task<IPersistentData> LoadAsync();
        new IPersistentData Save(object trans = null);
        new Task<IPersistentData> SaveAsync(object trans = null);

        /// <summary>
        /// 设置特定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>

        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        new IPersistentData Set<T>(T value, string key = null);

        new IPersistentData Set(Type objectType, object value, string key = null);
        /// <summary>
        /// 用数据源的格式去设置特定字段
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        new IPersistentData SetRaw(object value, string key = null);
        /// <summary>
        /// 移除某个字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new IPersistentData RemoveField(string key);
    }
}

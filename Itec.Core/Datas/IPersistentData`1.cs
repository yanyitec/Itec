using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Datas
{
    public interface IPersistentData<T>:IPersistentData,IPersistent
        where T : class
    {
        new IPersistentData<T> Load();
        new Task<IPersistentData<T>> LoadAsync();
        new IPersistentData<T> Save(object trans = null);
        new Task<IPersistentData<T>> SaveAsync(object trans = null);

        /// <summary>
        /// 设置特定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>

        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        IPersistentData<T> Set(T value, string key = null);

        new IPersistentData<T> Set(Type objectType, object value, string key = null);
        /// <summary>
        /// 用数据源的格式去设置特定字段
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        new IPersistentData<T> SetRaw(object value, string key = null);
        /// <summary>
        /// 移除某个字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new IPersistentData<T> RemoveField(string key);
    }
}

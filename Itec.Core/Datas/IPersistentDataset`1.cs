using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Datas
{
    public interface IPersistentDataset<T>:IWritableDataset<T>,IPersistentDataset, IPersistent
        where T : class
    {
        new IPersistentDataset<T> Load();
        new Task<IPersistentDataset<T>> LoadAsync();
        new IPersistentDataset<T> Save(object trans = null);
        new Task<IPersistentDataset<T>> SaveAsync(object trans = null);

        /// <summary>
        /// 设置特定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>

        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        new IPersistentDataset<T> Set(T value, string key = null);

        new IPersistentDataset<T> Set(Type objectType, object value, string key = null);
        /// <summary>
        /// 用数据源的格式去设置特定字段
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        new IPersistentDataset<T> SetRaw(object value, string key = null);
        /// <summary>
        /// 移除某个字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new IPersistentDataset<T> RemoveField(string key);
    }
}

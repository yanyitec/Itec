using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Itec.Datas
{
    public interface IPersistentDataset:IWritableDataset, IPersistent
    {
        new IPersistentDataset Load();
        new Task<IPersistentDataset> LoadAsync();
        new IPersistentDataset Save(object trans = null);
        new Task<IPersistentDataset> SaveAsync(object trans = null);

        /// <summary>
        /// 设置特定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>

        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        new IPersistentDataset Set<T>(T value, string key = null);

        new IPersistentDataset Set(Type objectType, object value, string key = null);
        /// <summary>
        /// 用数据源的格式去设置特定字段
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        new IPersistentDataset SetRaw(object value, string key = null);
        /// <summary>
        /// 移除某个字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new IPersistentDataset RemoveField(string key);
    }
}

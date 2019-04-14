using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Datas
{
    public interface IWritableDataset<T>:IWritableDataset
        where T : class
    {
        void Remove(T item);
        //void Append(object rawData);
        void Add(T obj, int at = -1);

        /// <summary>
        /// 设置特定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>

        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        IWritableDataset<T> Set(T value, string key = null);

        new IWritableDataset<T> Set(Type objectType, object value, string key = null);
        /// <summary>
        /// 用数据源的格式去设置特定字段
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        new IWritableDataset<T> SetRaw(object value, string key = null);
        /// <summary>
        /// 移除某个字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new IWritableDataset<T> RemoveField(string key);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Datas
{
    public interface IWritableDataset:IDataset,IWritableData
    {
        void Remove(int at);

        void Remove(object obj);
        //void Append(object rawData);
        void Add(object rawData,int at = -1);

        /// <summary>
        /// 设置特定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>

        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        new IWritableDataset Set<T>(T value, string key = null);

        new IWritableDataset Set(Type objectType, object value, string key = null);
        /// <summary>
        /// 用数据源的格式去设置特定字段
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        new IWritableDataset SetRaw(object value, string key = null);
        /// <summary>
        /// 移除某个字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new IWritableDataset RemoveField(string key);
    }
}

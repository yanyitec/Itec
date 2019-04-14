using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Datas
{
    public interface IWritableData<T>:IWritableData, IData<T>
        where T : class
    {
        void FromObject(T obj);

        /// <summary>
        /// 设置特定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>

        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        IWritableData<T> Set(T value, string key = null);

        new IWritableData<T> Set(Type objectType, object value, string key = null);
        /// <summary>
        /// 用数据源的格式去设置特定字段
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        new IWritableData<T> SetRaw(object value, string key = null);
        /// <summary>
        /// 移除某个字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        new IWritableData<T> RemoveField(string key);
    }
}


using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Datas
{
    /// <summary>
    /// 只读数据模型的抽象
    /// 可以得到
    /// </summary>
    public interface IData 
        : System.Dynamic.IDynamicMetaObjectProvider
        ,IEquatable<IData>
        //, IEnumerable<KeyValuePair<string, object>>
    {
        //bool HasChanges { get; }
        /// <summary>
        /// 获取某个特定字段的字符串值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string this[string key] { get; }
        /// <summary>
        /// 获取某个特定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key = null);
        /// <summary>
        /// 获取某个特定字段的字符串值
        /// 如果key==null,就获取整个对象的字符串序列化
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        string GetString(string key = null);
        /// <summary>
        /// 获取某个字段在数据源中的原始值
        /// 如果key为空，就获取整个对象的在数据源中的原始值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetRaw(string key = null);
        /// <summary>
        /// 获取某个字段的值，转化成特定的类型
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(Type objectType, string key = null);
        /// <summary>
        /// 获取整个对象的Json值
        /// </summary>
        /// <returns></returns>
        string ToJSON();

        IReadOnlyList<string> GetMemberNames();
    }
}

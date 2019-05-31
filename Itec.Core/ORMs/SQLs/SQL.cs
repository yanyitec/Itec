using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Itec.ORMs.SQLs
{
    public class SQL<T>
    {
        public SQL(string membersString, SQLBag<T> sqlBag)
        {
            this.SqlBag = sqlBag;
            this.Database = sqlBag.Database;
            this.DbSettings = this.Database.Settings;
            this.DbTrait = this.Database.Trait;
            this.DbClass = sqlBag.DbClass;
            this.FieldedProps = this.DbClass.FieldedProps;

            this.MembersString = membersString??string.Empty;
            if (string.IsNullOrEmpty(this.MembersString) || this.MembersString == "*")
            {
                this._AllowedProps = this.DbClass.FieldedProps;
                this.MembersString = "*";
            }
            else {
                var memberNames = MembersString.Split(',');
                var props = new Dictionary<string, IDbProperty>();
                _AllowedProps = props;
                foreach (var mName in memberNames)
                {
                    var memName = mName.Trim();
                    IDbProperty prop = null;
                    if (this.DbClass.FieldedProps.TryGetValue(memName, out prop))
                    {
                        props.Add(prop.Name, prop);
                    }

                }
                if (!props.ContainsKey(this.DbClass.PrimaryProperty.Name)) props.Add(this.DbClass.PrimaryProperty.Name,this.DbClass.PrimaryProperty);
            }
            

            

            this.Create = new Create<T>(this);
            this.Insert = new Insert<T>(this);
            this.Select = new Select<T>(this);
            this.Count = new Count<T>(this);
            this.Get = new Get<T>(this);
            this.GetById = new GetById<T>(this);
            this.Update = new Update<T>(this);
            this.Save = new Save<T>(this);
            this.Delete = new Delete<T>(this);
            this.DeleteById = new DeleteById<T>(this);
            //this.Select = new Select(model, membersString);
        }
        public Database Database { get; private set; }

        public IDbClass DbClass { get; private set; }
        public SQLBag<T> SqlBag { get; private set; }

        public DbSettings DbSettings { get; private set; }

        public DbTrait DbTrait { get; private set; }

        public IReadOnlyDictionary<string, IDbProperty> FieldedProps { get; private set; }

        public string MembersString { get; private set; }
        IReadOnlyDictionary<string, IDbProperty> _AllowedProps;
        public IReadOnlyDictionary<string, IDbProperty> AllowedProps
        {
            get
            {
                
                return _AllowedProps;
            }
        }

        public string Tablename( bool withSqlChar = false)
        {

            return this.SqlBag.Tablename(withSqlChar);
        }

        string _fields;
        public string SqlFields
        {
            get
            {
                if (_fields == null)
                {
                    lock (this)
                    {
                        this.BuildTbAndFieldsSql();
                    }
                }
                return _fields;
            }
        }
        string _tbAndFields;

        public string SqlTableAndFields {
            get
            {
                if (_tbAndFields == null)
                {
                    lock (this)
                    {
                        this.BuildTbAndFieldsSql();
                    }
                }
                return _tbAndFields;
            }
        }


        string _tbAndFieldsWithWhere;

        public string SqlTableAndFieldsWithWhere
        {
            get
            {
                if (_tbAndFieldsWithWhere == null)
                {
                    lock (this)
                    {
                        this.BuildTbAndFieldsSql();
                    }
                }
                return _tbAndFieldsWithWhere;
            }
        }

        string BuildTbAndFieldsSql()
        {
            var fields = string.Empty;
            foreach (var pair in this.AllowedProps)
            {
                var prop = pair.Value;
                var fieldname = this.DbTrait.SqlFieldname(prop.Field.Name);
                if (fields != string.Empty) fields += ",";
                fields += fieldname;
            }
            _fields = fields;
            _tbAndFields = $"SELECT {fields} FROM {this.Tablename(true)} ";
            _tbAndFieldsWithWhere = _tbAndFields + " WHERE ";

            return _tbAndFieldsWithWhere;
        }


        #region Command Builder
        Action<T, DbCommand> _ParametersBuilder;

        public void BuildParameters(DbCommand cmd, T data,IDbProperty constProp=null)
        {
            if (this.DbTrait.ParametricKind == SqlParametricKinds.Value) return;
            if (_ParametersBuilder == null)
            {
                lock (this)
                {
                    if (_ParametersBuilder == null) _ParametersBuilder = GenParametersBuilder(constProp);
                }
            }
            _ParametersBuilder(data, cmd);
        }
        Action<T, DbCommand> GenParametersBuilder(IDbProperty constProp)
        {
            ParameterExpression cmdExpr = Expression.Parameter(typeof(DbCommand), "cmd");
            ParameterExpression dataExpr = Expression.Parameter(typeof(T), "data");
            List<Expression> codes = new List<Expression>();
            List<ParameterExpression> locals = new List<ParameterExpression>();

            foreach (var pair in this.AllowedProps)
            {
                var prop = pair.Value;
                GenParam(prop.Field.Name, prop, dataExpr, cmdExpr, codes, locals);
            }
            if(constProp!=null && !this.AllowedProps.ContainsKey(constProp.Name) ){
                GenParam(constProp.Field.Name, constProp, dataExpr, cmdExpr, codes, locals);
            }

            var block = Expression.Block(locals, codes);
            var lamda = Expression.Lambda<Action<T, DbCommand>>(block, dataExpr, cmdExpr);
            return lamda.Compile();
        }
        static MethodInfo CreateParameterMethodInfo = typeof(DbCommand).GetMethod("CreateParameter");
        static MethodInfo AddParameterMethodInfo = typeof(DbParameterCollection).GetMethod("Add");
        void GenParam(string fname, IDbProperty prop, Expression dataExpr, Expression cmdExpr, List<Expression> codes, List<ParameterExpression> locals)
        {

            var paramExpr = Expression.Parameter(typeof(DbParameter), fname);
            locals.Add(paramExpr);
            codes.Add(Expression.Assign(paramExpr, Expression.Call(cmdExpr, CreateParameterMethodInfo)));
            codes.Add(Expression.Assign(Expression.PropertyOrField(paramExpr, "ParameterName"), Expression.Constant("@" + fname)));
            DbType dbType = prop.Field.DbType;
            codes.Add(Expression.Assign(Expression.PropertyOrField(paramExpr, "DbType"), Expression.Constant(dbType)));
            Expression valueExpr = Expression.PropertyOrField(dataExpr, prop.Name);



            if (prop.Field.Nullable)
            {

                if (prop.Nullable)
                {
                    valueExpr = Expression.Condition(
                        Expression.PropertyOrField(valueExpr, "HasValue")
                        , Expression.Convert(Expression.PropertyOrField(valueExpr, "Value"), typeof(object))
                        , Expression.Convert(Expression.Constant(DBNull.Value), typeof(object))
                    );
                }
                else if (prop.PropertyType == typeof(string))
                {

                    valueExpr = Expression.Condition(
                        Expression.Equal(valueExpr, Expression.Constant(null, typeof(string)))
                        , Expression.Convert(Expression.Constant(DBNull.Value), typeof(object))
                        , Expression.Convert(valueExpr, typeof(object))
                    );
                }

            }
            else
            {
                if (prop.Nullable)
                {
                    valueExpr = Expression.Condition(
                       Expression.PropertyOrField(valueExpr, "HasValue")
                       , Expression.Convert(Expression.PropertyOrField(valueExpr, "Value"), typeof(object))
                       , Expression.Convert(Expression.Constant(prop.DefaultValue), typeof(object))
                   );
                }
                else if (prop.PropertyType == typeof(string))
                {

                    valueExpr = Expression.Condition(
                        Expression.Equal(valueExpr, Expression.Constant(null, typeof(string)))
                        , Expression.Constant(string.Empty)
                        , valueExpr
                    );
                }


            }
            codes.Add(Expression.Assign(Expression.PropertyOrField(paramExpr, "Value"), Expression.Convert(valueExpr, typeof(object))));
            codes.Add(Expression.Call(Expression.Property(cmdExpr, "Parameters"), AddParameterMethodInfo, paramExpr));
            //DbParameter par;
            //par.Value
        }

        #endregion


        public Create<T> Create { get; private set; }
        public Insert<T> Insert { get; private set; }
        public Select<T> Select { get; private set; }

        public Count<T> Count { get; private set; }

        public GetById<T> GetById { get; private set; }

        public Get<T> Get { get; private set; }

        public Update<T> Update { get; private set; }

        public Save<T> Save{get;private set;}

        public Delete<T> Delete{get;private set;}

        public DeleteById<T> DeleteById { get; private set; }

        

        public static string SafeString(string str)
        {
            return str.Replace("'", "''");//.Replace("\n","\\n").Replace("\r", "\\r");
        }

        public static string SqlValue(object value, bool nullable = false, object defaultValue = null)
        {
            if (value == null)
            {
                if (nullable) return "NULL";
                else value = defaultValue ?? "";
            }
            var t = value.GetType();
            if (t == typeof(DateTime)) return "'1790-1-1'";
            if (t.IsClass) return "'" + SafeString(value.ToString()) + "'";
            return value.ToString();

        }
    }
}

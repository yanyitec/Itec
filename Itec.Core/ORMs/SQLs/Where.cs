
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Itec.ORMs.SQLs
{
    public class Where<T>
    {
        
        public SQL<T> Sql { get; private set; }

        public Where(SQL<T> sql)
        {
            this.Sql = sql;

        }



        
        //public int noSeed { get; private set; }



        public string SqlWhere(Expression exp, DbCommand cmd, WhereOpts opts=null)
        {
            if (exp == null) return null;
            if (opts == null) opts = new WhereOpts();
            BinaryExpression bExp = null;
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return SqlWhere(((LambdaExpression)exp).Body,cmd,opts);
                
                case ExpressionType.AndAlso:
                    bExp = exp as BinaryExpression;
                    return "(" + SqlWhere(bExp.Left, cmd,opts)
                         + " AND "
                         + SqlWhere(bExp.Right, cmd ,opts )
                         + ")";
                case ExpressionType.OrElse:
                    bExp = exp as BinaryExpression;
                    return "(" + SqlWhere(bExp.Left, cmd ,opts )
                         + " OR "
                         + SqlWhere(bExp.Right, cmd ,opts )
                         + ")";
                case ExpressionType.Equal:
                    bExp = exp as BinaryExpression;
                    return "(" + SqlWhere(bExp.Left, cmd ,opts )
                         + " = "
                         + SqlWhere(bExp.Right, cmd ,opts )
                         + ")";
                case ExpressionType.NotEqual:
                    bExp = exp as BinaryExpression;
                    return "(" + SqlWhere(bExp.Left, cmd ,opts )
                         + " <> "
                         + SqlWhere(bExp.Right, cmd ,opts )
                         + ")";
                case ExpressionType.GreaterThan:
                    bExp = exp as BinaryExpression;
                    return "(" + SqlWhere(bExp.Left, cmd ,opts )
                         + " > "
                         + SqlWhere(bExp.Right, cmd ,opts )
                         + ")";
                case ExpressionType.GreaterThanOrEqual:
                    bExp = exp as BinaryExpression;
                    return "(" + SqlWhere(bExp.Left,cmd ,opts )
                         + " >= "
                         + SqlWhere(bExp.Right, cmd ,opts )
                         + ")";
                case ExpressionType.LessThan:
                    bExp = exp as BinaryExpression;
                    return "(" + SqlWhere(bExp.Left, cmd ,opts )
                         + " < "
                         + SqlWhere(bExp.Right, cmd ,opts )
                         + ")";
                case ExpressionType.LessThanOrEqual:
                    bExp = exp as BinaryExpression;
                    return "(" + SqlWhere(bExp.Left, cmd ,opts )
                         + " <= "
                         + SqlWhere(bExp.Right, cmd ,opts )
                         + ")";
                case ExpressionType.Add:
                    bExp = exp as BinaryExpression;
                    return "(" + SqlWhere(bExp.Left, cmd, opts)
                         + " + "
                         + SqlWhere(bExp.Right, cmd, opts)
                         + ")";
                case ExpressionType.Decrement:
                    bExp = exp as BinaryExpression;
                    return "(" + SqlWhere(bExp.Left, cmd, opts)
                         + " - "
                         + SqlWhere(bExp.Right, cmd, opts)
                         + ")";
                case ExpressionType.Multiply:
                    bExp = exp as BinaryExpression;
                    return "(" + SqlWhere(bExp.Left, cmd, opts)
                         + " * "
                         + SqlWhere(bExp.Right, cmd, opts)
                         + ")";
                case ExpressionType.Divide:
                    bExp = exp as BinaryExpression;
                    return "(" + SqlWhere(bExp.Left, cmd, opts)
                         + " / "
                         + SqlWhere(bExp.Right, cmd, opts)
                         + ")";
                case ExpressionType.MemberAccess:
                    var m = exp as MemberExpression;
                    var member = this.Sql.AllowedProps.Values.FirstOrDefault(p=>p.Name==m.Member.Name);
                    opts.LastProp = member;
                    return this.Sql.DbTrait.SqlFieldname(member.Field.Name);
                case ExpressionType.Convert:
                    var cexp = exp as UnaryExpression;
                    return SqlWhere(cexp.Operand, cmd, opts);

                case ExpressionType.Call:
                    var method = exp as MethodCallExpression;
                    switch (method.Method.Name)
                    {
                        case "Contains":
                            opts.ValueEmbeded = true;
                            return SqlWhere(method.Object, cmd ,opts )
                                + " LIKE ('%"
                                + SqlWhere(method.Arguments[0], cmd ,opts )
                                + "%')";
                        case "StartsWith":
                            opts.ValueEmbeded = true;
                            return SqlWhere(method.Object, cmd ,opts )
                                + " LIKE ('"
                                + SqlWhere(method.Arguments[0], cmd ,opts )
                                + "%')";
                        case "EndsWith":
                            opts.ValueEmbeded = true;
                            return SqlWhere(method.Object, cmd ,opts )
                                + " LIKE ('%"
                                + SqlWhere(method.Arguments[0], cmd ,opts )
                                + "')";
                        default:
                            throw new InvalidExpressionException("无法支持的表达式");
                    }
                case ExpressionType.Constant:
                    var cst = (exp as ConstantExpression);
                    if (opts.ValueEmbeded) {
                        opts.ValueEmbeded = false;
                        return cst.Value.ToString().Replace("'","''");
                    }
                    var par = cmd.CreateParameter();
                    par.ParameterName = "@_WP" + (++opts.NoSeed).ToString();
                    par.Value = cst.Value;
                    
                    par.DbType = opts.LastProp.Field.DbType;
                    cmd.Parameters.Add(par);
                    return par.ParameterName;
                default:
                    throw new InvalidExpressionException("无法支持的表达式");


            }
        }
    }
}

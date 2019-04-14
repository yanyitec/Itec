
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



        public string SqlWhere(Expression exp, DbCommand cmd,ref IDbProperty prop,int noSeed)
        {
            if (exp == null) return null;
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return SqlWhere(((LambdaExpression)exp).Body,cmd,ref prop,noSeed);
                case ExpressionType.AndAlso:
                    var andAlso = exp as BinaryExpression;
                    return "(" + SqlWhere(andAlso.Left, cmd,ref prop, noSeed)
                         + " AND "
                         + SqlWhere(andAlso.Right, cmd ,ref prop ,noSeed)
                         + ")";
                case ExpressionType.OrElse:
                    var orElse = exp as BinaryExpression;
                    return "(" + SqlWhere(orElse.Left, cmd ,ref prop ,noSeed)
                         + " OR "
                         + SqlWhere(orElse.Right, cmd ,ref prop ,noSeed)
                         + ")";
                case ExpressionType.Equal:
                    var eq = exp as BinaryExpression;
                    return "(" + SqlWhere(eq.Left, cmd ,ref prop ,noSeed)
                         + " = "
                         + SqlWhere(eq.Right, cmd ,ref prop ,noSeed)
                         + ")";
                case ExpressionType.NotEqual:
                    var neq = exp as BinaryExpression;
                    return "(" + SqlWhere(neq.Left, cmd ,ref prop ,noSeed)
                         + " <> "
                         + SqlWhere(neq.Right, cmd ,ref prop ,noSeed)
                         + ")";
                case ExpressionType.GreaterThan:
                    var gt = exp as BinaryExpression;
                    return "(" + SqlWhere(gt.Left, cmd ,ref prop ,noSeed)
                         + " > "
                         + SqlWhere(gt.Right, cmd ,ref prop ,noSeed)
                         + ")";
                case ExpressionType.GreaterThanOrEqual:
                    var gte = exp as BinaryExpression;
                    return "(" + SqlWhere(gte.Left,cmd ,ref prop ,noSeed)
                         + " >= "
                         + SqlWhere(gte.Right, cmd ,ref prop ,noSeed)
                         + ")";
                case ExpressionType.LessThan:
                    var lt = exp as BinaryExpression;
                    return "(" + SqlWhere(lt.Left, cmd ,ref prop ,noSeed)
                         + " < "
                         + SqlWhere(lt.Right, cmd ,ref prop ,noSeed)
                         + ")";
                case ExpressionType.LessThanOrEqual:
                    var lte = exp as BinaryExpression;
                    return "(" + SqlWhere(lte.Left, cmd ,ref prop ,noSeed)
                         + " <= "
                         + SqlWhere(lte.Right, cmd ,ref prop ,noSeed)
                         + ")";
                case ExpressionType.MemberAccess:
                    var m = exp as MemberExpression;
                    var member = this.Sql.AllowedProps.Values.FirstOrDefault(p=>p.Name==m.Member.Name);
                    prop = member;
                    return this.Sql.DbTrait.SqlFieldname(member.Field.Name);
                    
                case ExpressionType.Call:
                    var method = exp as MethodCallExpression;
                    switch (method.Method.Name)
                    {
                        case "Contains":
                            return SqlWhere(method.Object, cmd ,ref prop ,noSeed)
                                + " LIKE ('%"
                                + SqlWhere(method.Arguments[0], cmd ,ref prop ,noSeed)
                                + "%')";
                        case "StartWith":
                            return SqlWhere(method.Object, cmd ,ref prop ,noSeed)
                                + " LIKE ('"
                                + SqlWhere(method.Arguments[0], cmd ,ref prop ,noSeed)
                                + "%')";
                        case "EndWith":
                            return SqlWhere(method.Object, cmd ,ref prop ,noSeed)
                                + " LIKE ('%"
                                + SqlWhere(method.Arguments[0], cmd ,ref prop ,noSeed)
                                + "')";
                        default:
                            throw new InvalidExpressionException("无法支持的表达式");
                    }
                case ExpressionType.Constant:
                    var cst = (exp as ConstantExpression);
                    var par = cmd.CreateParameter();
                    par.ParameterName = "@p_" + (++noSeed).ToString();
                    par.Value = cst.Value;
                    
                    par.DbType = prop.Field.DbType;
                    cmd.Parameters.Add(par);
                    return par.ParameterName;
                default:
                    throw new InvalidExpressionException("无法支持的表达式");


            }
        }
    }
}

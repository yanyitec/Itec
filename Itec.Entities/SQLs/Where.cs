using Itec.Entities.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;

namespace Itec.Entities.SQLs
{
    public class Where
    {
        
        public Where(EntityModel model)
        {
            this.Model = model;
            this.DbSettings = model.Database.Settings;
            this.Trait = model.Database.Trait;
            
        }

        

        public EntityModel Model { get; private set; }


        public DbSettings DbSettings { get; private set; }

        public DbTrait Trait { get; private set; }
        public int NoSeed { get; private set; }

        

        public string BuildSql(Expression exp, DbCommand cmd) {
            switch (exp.NodeType) {
                case ExpressionType.AndAlso:
                    var andAlso = exp as BinaryExpression;
                    return "(" + BuildSql(andAlso.Left, cmd)
                         + " AND "
                         + BuildSql(andAlso.Right, cmd)
                         +")";
                case ExpressionType.OrElse:
                    var orElse = exp as BinaryExpression;
                    return "(" + BuildSql(orElse.Left, cmd)
                         + " OR "
                         + BuildSql(orElse.Right, cmd)
                         + ")";
                case ExpressionType.Equal:
                    var eq = exp as BinaryExpression;
                    return "(" + BuildSql(eq.Left, cmd)
                         + " = "
                         + BuildSql(eq.Right, cmd)
                         + ")";
                case ExpressionType.NotEqual:
                    var neq = exp as BinaryExpression;
                    return "(" + BuildSql(neq.Left, cmd)
                         + " <> "
                         + BuildSql(neq.Right, cmd)
                         + ")";
                case ExpressionType.GreaterThan:
                    var gt = exp as BinaryExpression;
                    return "(" + BuildSql(gt.Left, cmd)
                         + " > "
                         + BuildSql(gt.Right, cmd)
                         + ")";
                case ExpressionType.GreaterThanOrEqual:
                    var gte = exp as BinaryExpression;
                    return "(" + BuildSql(gte.Left, cmd)
                         + " >= "
                         + BuildSql(gte.Right, cmd)
                         + ")";
                case ExpressionType.LessThan:
                    var lt = exp as BinaryExpression;
                    return "(" + BuildSql(lt.Left, cmd)
                         + " < "
                         + BuildSql(lt.Right, cmd)
                         + ")";
                case ExpressionType.LessThanOrEqual:
                    var lte = exp as BinaryExpression;
                    return "(" + BuildSql(lte.Left, cmd)
                         + " <= "
                         + BuildSql(lte.Right, cmd)
                         + ")";
                case ExpressionType.MemberAccess:
                    var m = exp as MemberExpression;
                    return this.Trait.MakeSqlFieldname(m.Member.Name);
                case ExpressionType.Call:
                    var method = exp as MethodCallExpression;
                    switch (method.Method.Name) {
                        case "Contains":
                            return BuildSql(method.Object, cmd)
                                + " LIKE ('%"
                                + BuildSql(method.Arguments[0],cmd)
                                + "%')";
                        case "StartWith":
                            return BuildSql(method.Object, cmd)
                                + " LIKE ('"
                                + BuildSql(method.Arguments[0], cmd)
                                + "%')";
                        case "EndWith":
                            return BuildSql(method.Object, cmd)
                                + " LIKE ('%"
                                + BuildSql(method.Arguments[0], cmd)
                                + "')";
                        default:
                            throw new InvalidExpressionException("无法支持的表达式");
                    }
                case ExpressionType.Constant:
                    var cst = (exp as ConstantExpression);
                    var par = cmd.CreateParameter();
                    par.ParameterName = "@p_" + (++NoSeed).ToString();
                    par.Value = cst.Value;
                    DbType dbtype = DbType.String;
                    this.Trait.DataTypeMaps.TryGetValue(cst.Value.GetType().GUID,out dbtype);
                    par.DbType = dbtype;
                    cmd.Parameters.Add(par);
                    return par.ParameterName;
                default:
                    throw new InvalidExpressionException("无法支持的表达式");


            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json.Schema;

namespace Itec
{
    public static class FilterableExtensions
    {
        
        

        

        
        public static IFilterable<T> Ascending<T>(this IFilterable<T>  self,Expression<Func<T, object>> expr)
        {
            self.AscendingExpression = expr;
            return self;
        }

       
        public static IFilterable<T> Descending<T>(this IFilterable<T> self, Expression<Func<T, object>> expr)
        {
            self.DescendingExpression = expr;
            return self;
        }

        

        public static IFilterable<T> Take<T>(this IFilterable<T> self, int size) {
            self.TakeCount = size;
            return self;
        }
        
        public static IFilterable<T> Skip<T>(this IFilterable<T> self, int count)
        {
            self.SkipCount = count;
            return self ;
        }

        public static IFilterable<T> Page<T>(this IFilterable<T> self,  int index, int size = 10) {
            if (index <= 0) index = 1;
            if (size <= 2) size = 2;
            self.TakeCount = size;
            self.SkipCount = (index - 1) * size;
            return self;
        }



        


        public static Expression<Func<T,bool>> AndAlso<T>(this Expression<Func<T, bool>> self,Expression<Func<T, bool>> criteria)
        {
            if (criteria == null) return self;
            if (self == null)
            {
                return criteria;
            }
            else
            {
                //this._Expression = System.Linq.Expressions.Expression.AndAlso(this._Expression, criteria);
                return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(self.Body, ReplaceParameter<T>( criteria.Body, criteria.Parameters[0])), self.Parameters[0]);
            }
        }

        public static IFilterable<T> AndAlso<T>(this IFilterable<T> self, Expression<Func<T, bool>> criteria)
        {
            if (criteria == null) return self;
            if (self.QueryExpression == null) { self.QueryExpression = criteria; return self; }
            self.QueryExpression= Expression.Lambda<Func<T, bool>>(Expression.AndAlso(self.QueryExpression.Body, ReplaceParameter<T>(criteria.Body, criteria.Parameters[0])), self.QueryParameter);
            return self;
        }



        public static Expression<Func<T, bool>>  OrElse<T>(this Expression<Func<T, bool>> self, Expression<Func<T, bool>> criteria)
        {
            if (criteria == null) return self;
            if (self == null)
            {
                return criteria;
            }
            else
            {
                //this._Expression = System.Linq.Expressions.Expression.OrElse(this._Expression, criteria);
                return Expression.Lambda<Func<T,bool>>(Expression.OrElse(self.Body, ReplaceParameter<T>(criteria.Body, criteria.Parameters[0])),self.Parameters[0]);
            }
        }

        

        public static Expression ReplaceParameter<T>( Expression expr, ParameterExpression param)
        {
            if (expr == param) return param;
            BinaryExpression bExpr = null;
            UnaryExpression uExpr = null;
            switch (expr.NodeType)
            {
                case ExpressionType.Lambda:
                    var lamda = (expr as LambdaExpression);
                    return ReplaceParameter<T>(lamda.Body,lamda.Parameters[0]);
                case ExpressionType.Constant:
                    return expr;
                case ExpressionType.And:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.And(ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.Add:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Add(ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.AndAlso:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.OrElse( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.MemberAccess:
                    var member = expr as MemberExpression;
                    return System.Linq.Expressions.Expression.MakeMemberAccess( ReplaceParameter<T>(member.Expression,param),member.Member);
                case ExpressionType.Call:
                    var call = expr as MethodCallExpression;
                    var list = new List<Expression>();
                    foreach (var arg in call.Arguments)
                    {
                        list.Add( ReplaceParameter<T>(arg, param));
                    }
                    return System.Linq.Expressions.Expression.Call( ReplaceParameter<T>(call.Object, param), call.Method, list);
                case ExpressionType.Convert:
                    uExpr = expr as UnaryExpression;
                    return System.Linq.Expressions.Expression.Convert( ReplaceParameter<T>(uExpr.Operand, param), uExpr.Type);
                case ExpressionType.Divide:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Divide( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.Equal:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Equal( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.GreaterThan:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.GreaterThan( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.GreaterThanOrEqual:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.GreaterThanOrEqual( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.LessThan:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.LessThan( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.LessThanOrEqual:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.LessThan( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.LeftShift:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.LeftShift( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.Multiply:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Multiply( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.Negate:
                    uExpr = expr as UnaryExpression;
                    return System.Linq.Expressions.Expression.Negate( ReplaceParameter<T>(uExpr.Operand, param));
                case ExpressionType.Not:
                    uExpr = expr as UnaryExpression;
                    return System.Linq.Expressions.Expression.Not( ReplaceParameter<T>(uExpr.Operand, param));
                case ExpressionType.NotEqual:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.NotEqual( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.Or:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Or( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.OrElse:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.OrElse( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.Power:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Power( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.RightShift:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.RightShift( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));
                case ExpressionType.Subtract:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Subtract( ReplaceParameter<T>(bExpr.Left, param), ReplaceParameter<T>(bExpr.Right, param));

            }
            throw new NotSupportedException();
        }

       
    }
}
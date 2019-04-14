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


        



        public static IFilterable<T> AndAlso<T>(this IFilterable<T> self,Expression<Func<T, bool>> criteria)
        {
            if (criteria == null) return self;
            if (self.QueryExpression == null)
            {
                self.QueryExpression = criteria;
            }
            else
            {
                //this._Expression = System.Linq.Expressions.Expression.AndAlso(this._Expression, criteria);
                self.QueryExpression = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(self.QueryExpression.Body, Convert<T>(self, criteria.Body, criteria.Parameters[0])), self.QueryParameter);
            }
            return self;
        }



        public static IFilterable<T> OrElse<T>(this IFilterable<T> self, Expression<Func<T, bool>> criteria)
        {
            if (criteria == null) return self;
            if (self.QueryExpression == null)
            {
                self.QueryExpression = criteria;
            }
            else
            {
                //this._Expression = System.Linq.Expressions.Expression.OrElse(this._Expression, criteria);
                self.QueryExpression =Expression.Lambda<Func<T,bool>>(Expression.OrElse(self.QueryExpression.Body, Convert<T>(self,criteria.Body, criteria.Parameters[0])),self.QueryParameter);
            }
            return self;
        }

        

        public static Expression Convert<T>(this IFilterable<T> self, Expression expr, ParameterExpression param)
        {
            if (expr == param) return self.QueryParameter;
            BinaryExpression bExpr = null;
            UnaryExpression uExpr = null;
            switch (expr.NodeType)
            {
                case ExpressionType.Lambda:
                    var lamda = (expr as LambdaExpression);
                    return Convert<T>(self,lamda.Body,lamda.Parameters[0]);
                case ExpressionType.Constant:
                    return expr;
                case ExpressionType.And:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.And(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.Add:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Add(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.AndAlso:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.OrElse(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.MemberAccess:
                    var member = expr as MemberExpression;
                    return System.Linq.Expressions.Expression.MakeMemberAccess(Convert<T>(self,member.Expression,param),member.Member);
                case ExpressionType.Call:
                    var call = expr as MethodCallExpression;
                    var list = new List<Expression>();
                    foreach (var arg in call.Arguments)
                    {
                        list.Add(Convert<T>(self,arg, param));
                    }
                    return System.Linq.Expressions.Expression.Call(Convert<T>(self,call.Object, param), call.Method, list);
                case ExpressionType.Convert:
                    uExpr = expr as UnaryExpression;
                    return System.Linq.Expressions.Expression.Convert(Convert<T>(self,uExpr.Operand, param), uExpr.Type);
                case ExpressionType.Divide:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Divide(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.Equal:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Equal(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.GreaterThan:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.GreaterThan(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.GreaterThanOrEqual:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.GreaterThanOrEqual(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.LessThan:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.LessThan(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.LessThanOrEqual:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.LessThan(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.LeftShift:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.LeftShift(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.Multiply:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Multiply(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.Negate:
                    uExpr = expr as UnaryExpression;
                    return System.Linq.Expressions.Expression.Negate(Convert<T>(self,uExpr.Operand, param));
                case ExpressionType.Not:
                    uExpr = expr as UnaryExpression;
                    return System.Linq.Expressions.Expression.Not(Convert<T>(self,uExpr.Operand, param));
                case ExpressionType.NotEqual:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.NotEqual(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.Or:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Or(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.OrElse:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.OrElse(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.Power:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Power(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.RightShift:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.RightShift(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));
                case ExpressionType.Subtract:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Subtract(Convert<T>(self,bExpr.Left, param), Convert<T>(self,bExpr.Right, param));

            }
            throw new NotSupportedException();
        }

       
    }
}
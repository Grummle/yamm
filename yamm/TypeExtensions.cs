using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace yamm
{
    public static class TypeExtensions
    {
        public static bool IsNullableType(this Type type)
        {
            return ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        public static bool IsStruct(this Type type)
        {
            return (type.IsValueType && !type.IsEnum && !type.IsPrimitive);
        }
    }

    public static class ForType<T>
    {
        public static PropertyInfo GetProperty<TValue>(Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return (PropertyInfo)((MemberExpression)body).Member;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace yamm.Mapping
{
    public interface IMap
    {
        string FromPropertyName { get; }
        string ToPropertyName { get; }
        Expression AccessFromProperty(Expression fromExpression);
        Expression AccessToProperty(Expression toExpression);
        Expression Assign(Expression toParamater, Expression fromParamater);
        Expression ValidateFrom(Expression param);
        IList<PropertyInfo> FromComponents { get; }
        IList<PropertyInfo> ToComponents { get; }
    }
}

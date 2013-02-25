using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace yamm.Mapping
{

    public class NullableMap : IMap
    {
        private Lazy<IList<PropertyInfo>> _fromInfo = new Lazy<IList<PropertyInfo>>(() => new List<PropertyInfo>());
        private Lazy<IList<PropertyInfo>> _toInfo = new Lazy<IList<PropertyInfo>>(() => new List<PropertyInfo>());

        public IList<PropertyInfo> FromComponents { get { return _fromInfo.Value; } }
        public IList<PropertyInfo> ToComponents { get { return _toInfo.Value; } }

        public string FromPropertyName { get { return String.Join(".", FromComponents.Select(x => x.Name)); } }
        public string ToPropertyName { get { return String.Join(".", ToComponents.Select(x => x.Name)); } }

        public NullableMap(PropertyInfo fromInfo, PropertyInfo toInfo)
        {
            FromComponents.Add(fromInfo);
            ToComponents.Add(toInfo);
        }

        public NullableMap() { }

        public Expression AccessFromProperty(Expression fromExpression)
        {
            Expression output = null;

            foreach (var property in FromComponents)
            {
                if (output.IsNull()) output = fromExpression;
                output = Expression.Property(output, property);
            }
            output = Expression.Convert(output, ToComponents.Last().PropertyType);
            return output;
        }

        public Expression AccessToProperty(Expression toExpression)
        {
            Expression output = null;

            foreach (var property in ToComponents)
            {
                if (output.IsNull()) output = toExpression;
                output = Expression.Property(output, property);
            }

            return output;
        }

        public Expression Assign(Expression toParamater, Expression fromParamater)
        {
            return Expression.Assign(AccessToProperty(toParamater), AccessFromProperty(fromParamater));
        }

        public Expression ValidateFrom(Expression param)
        {
            var returnTarget = Expression.Label(typeof(bool));

            var falseValue = Expression.Constant(false);
            var trueValue = Expression.Constant(true);

            if (FromComponents.Count == 1)
                return Expression.Label(returnTarget, trueValue);

            var returnTrue = Expression.Return(returnTarget, trueValue);

            var nullConstant = Expression.Constant(null);

            Expression ifTree = null;

            var propertyAccessors = new List<Expression> { Expression.Property(param, FromComponents.First()) };

            foreach (var property in FromComponents.Skip(1))
            {
                propertyAccessors.Add(Expression.Property(propertyAccessors.Last(), property));
            }

            propertyAccessors.Reverse();

            foreach (var accessor in propertyAccessors)
            {
                var neq = Expression.Not(Expression.Equal(accessor, nullConstant));
                if (ifTree == null)
                {
                    var ifExp = Expression.IfThen(neq, returnTrue);
                    ifTree = ifExp;
                }
                else
                {
                    ifTree = Expression.IfThen(neq, ifTree);
                }
            }

            return Expression.Block(ifTree, Expression.Label(returnTarget, falseValue));
        }
    }
}
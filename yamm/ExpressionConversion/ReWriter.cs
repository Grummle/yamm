using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using yamm.Mapping;

namespace yamm.ExpressionConversion
{
    public class Converter<TTo>
    {
        class ConversionVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _newParameter;
            private readonly ParameterExpression _oldParameter;
            public IEnumerable<IMap> Maps { get; set; }

            public ConversionVisitor(ParameterExpression newParameter, ParameterExpression oldParameter)
            {
                this._newParameter = newParameter;
                this._oldParameter = oldParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return _newParameter; // replace all old param references with new ones
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression != _oldParameter) // if instance is not old parameter - do nothing
                    return base.VisitMember(node);

                var newObj = Visit(node.Expression);

                var map = Maps.First(x => x.ToPropertyName == node.Member.Name);
                return map.AccessFromProperty(newObj);
            }
        }

        public static Expression<Func<TTo, TR>> Convert<TFrom, TR>(Expression<Func<TFrom, TR>> e,IEnumerable<IMap> maps)
        {
            var oldParameter = e.Parameters[0];
            var newParameter = Expression.Parameter(typeof(TTo), oldParameter.Name);
            var converter = new ConversionVisitor(newParameter, oldParameter);

            converter.Maps = maps;
            var newBody = converter.Visit(e.Body);
            return Expression.Lambda<Func<TTo, TR>>(newBody, newParameter);
        }
    }

}

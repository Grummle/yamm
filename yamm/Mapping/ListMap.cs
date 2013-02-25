using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using yamm.Matching;

namespace yamm.Mapping
{
    public class ListMap : IMap
    {
        private IList<PropertyInfo> _toComponents = new List<PropertyInfo>();
        private IList<PropertyInfo> _fromComponents = new List<PropertyInfo>();

        public string FromPropertyName { get { return _fromComponents.Select(x => x.Name).Join("."); } }
        public string ToPropertyName { get { return _toComponents.Select(x => x.Name).Join("."); } }

        public IList<PropertyInfo> FromComponents { get { return _fromComponents; } }
        public IList<PropertyInfo> ToComponents { get { return _toComponents; } }

        public ListMap() { }

        public ListMap(PropertyInfo fromPropertyInfo, PropertyInfo toPropertyInfo)
        {
            _fromComponents.Add(fromPropertyInfo);
            _toComponents.Add(toPropertyInfo);
        }

        public Expression AccessFromProperty(Expression fromExpression)
        {
            Expression output = null;

            foreach (var property in FromComponents)
            {
                if (output.IsNull()) output = fromExpression;
                output = Expression.Property(output, property);
            }

            if (ToComponents.Last().IsNullableType())
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
            MethodInfo method = typeof(ListMap).GetMethod("Frankenstien");
            var generic = method.MakeGenericMethod(FromComponents.Last().PropertyType.GetGenericArguments()[0], ToComponents.Last().PropertyType.GetGenericArguments()[0]);

            Expression exp = (Expression)generic.Invoke(this, null);

            var assign = Expression.Assign(AccessToProperty(toParamater), Expression.Invoke(exp, AccessFromProperty(fromParamater)));

            return assign;
        }

        public Expression Frankenstien<E, M>() where M : new()
        {
            var mapper = new yamm.Mapper.Mapper<E, M>(new List<IMatcher>
                                                             {
                                                                 new BasicMatcher(),
                                                                 new NullableMatcher(),
                                                                 new FlattenMatcher(),
                                                                 new ListMatcher()
                                                             });
            Expression<Func<IList<E>, IList<M>>> converter = e => e.Select(x => mapper.Map(x, new M())).ToList();

            return converter;
        }

        public Expression ValidateFrom(Expression param)
        {
            var returnTarget = Expression.Label(typeof(bool));

            var falseValue = Expression.Constant(false);
            var trueValue = Expression.Constant(true);

            var returnTrue = Expression.Return(returnTarget, trueValue);

            var nullConstant = Expression.Constant(null);

            Expression ifTree = null;

            var propertyAccessors = new List<MemberExpression> { Expression.Property(param, FromComponents.First()) };

            foreach (var property in FromComponents.Skip(1))
            {
                propertyAccessors.Add(Expression.Property(propertyAccessors.Last(), property));
            }

            propertyAccessors.Reverse();

            foreach (var accessor in propertyAccessors)
            {

                Console.WriteLine(((PropertyInfo)accessor.Member).PropertyType.IsStruct());
                Console.WriteLine(((PropertyInfo)accessor.Member).PropertyType.Name);
                Console.WriteLine(((PropertyInfo)accessor.Member).Name);

                Expression neq = null;

                if (((PropertyInfo)accessor.Member).PropertyType.IsStruct())
                    neq = trueValue;
                else
                    neq = Expression.Not(Expression.Equal(accessor, nullConstant));

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

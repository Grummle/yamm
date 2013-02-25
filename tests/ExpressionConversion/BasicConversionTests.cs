using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Should;
using yamm.Mapping;

namespace tests.ExpressionConversion
{
    [TestFixture]
    public class BasicConversionTests
    {
        [Test]
        public void Should_Convert_Basic_Member_Access()
        {
            var basicMap = new BasicMap(typeof (Entity).GetProperty("Number"), typeof (Model).GetProperty("number"));
            var listMap = new ListMap(typeof (Entity).GetProperty("Nums"), typeof (Model).GetProperty("nums"));
            var entity = new Entity { Number = 1 ,Nums = new []{1,2,3}};
            var entity2 = new Entity {Number = 2,Nums = new []{4,5,6}};

            Expression<Func<Model, bool>> exp = x => x.number == 1 && x.nums.All(y=>y>3);

            //Expression<Func<A, int>> f = x => x.Value;
            var f2 = Converter<Entity>.Convert(exp,new IMap[]{basicMap,listMap});

            //f2.Compile().Invoke(entity).ShouldBeTrue();
            new[] {entity, entity2}.AsEnumerable().Where(f2.Compile()).Count().ShouldEqual(1);
        }
    }


    public class Entity
    {
        public int Number { get; set; }
        public IList<int> Nums { get; set; }
    }

    public class Model
    {
        public int number { get; set; }
        public IList<int> nums { get; set; } 
    }



    class Converter<TTo>
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

    class A
    {
        public int Value { get; set; }
    }

    class B
    {
        public int Value { get; set; }
    }


}

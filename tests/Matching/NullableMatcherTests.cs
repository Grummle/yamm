using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Should;
using yamm;
using yamm.Matching;

namespace tests.Matching
{
    [TestFixture]
    public class NullableMatcherTests
    {
        private NullableMatcher _matcher;

        [SetUp]
        public void Setup()
        {
            _matcher = new NullableMatcher();
        }


        [Test]
        public void Should_Create_Expressions_For_Property_Access_For_Nullable()
        {
            var be = new BasicEntity { Id = Guid.NewGuid() };
            var sm = new BasicModel { id = Guid.NewGuid() };

            var maps = _matcher.Match(be.GetType().GetProperties(), sm.GetType().GetProperties());
            maps.Each(x => x.FromPropertyName.WriteLine());

            maps.Count.ShouldEqual(1);

            maps.First().FromComponents.Count.ShouldEqual(1);
            maps.First().FromComponents.First().Name.ShouldEqual("Id");

            maps.First().ToComponents.Count.ShouldEqual(1);
            maps.First().ToComponents.First().Name.ShouldEqual("id");

            CreateLambda<BasicEntity, Guid?>(maps.First(x => x.FromPropertyName == "Id").AccessFromProperty)(be).ShouldEqual(be.Id);
            CreateLambda<BasicModel, Guid?>(maps.First(x => x.FromPropertyName == "Id").AccessToProperty)(sm).ShouldEqual(sm.id);
        }

        private Func<T, R> CreateLambda<T, R>(Func<Expression, Expression> accessor)
        {
            var param = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, R>>(accessor(param), param).Compile();
        }

        public class BasicEntity
        {
            public Guid Id { get; set; }
        }

        public class BasicModel
        {
            public Guid? id { get; set; }
        }
    }
}

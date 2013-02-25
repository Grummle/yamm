using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Should;
using yamm.Matching;

namespace tests.Matching
{
    [TestFixture]
    public class FlattenMatcherTests
    {
        private FlattenMatcher _matcher;

        [SetUp]
        public void Setup()
        {
            _matcher = new FlattenMatcher();
        }

        [Test]
        public void Should_Create_Expressions_For_Property_Access()
        {
            var be = new BasicEntity
            {
                Name = "Michael",
                SubEntity = new SubEntity { SubName = "Mini Michael" }
            };
            var sm = new BasicModel
            {
                name = "Not Michael",
                subEntitySubName = "Not Mini Michael"
            };

            var maps = _matcher.Match(be.GetType().GetProperties(), sm.GetType().GetProperties());

            maps.Count(x => x.ToPropertyName == "subEntitySubName").ShouldEqual(1, "No Maps Produced");

            var map = maps.First(x => x.ToPropertyName == "subEntitySubName");

            map.FromComponents.Count().ShouldEqual(2);
            map.ToComponents.Count.ShouldEqual(1);

            map.FromComponents.First().Name.ShouldEqual("SubEntity");
            map.FromComponents.Skip(1).First().Name.ShouldEqual("SubName");

        }

        [Test]
        public void Should_Create_Expressions_For_Property_Access2()
        {
            var be = new BasicEntity
            {
                SubEntity = new SubEntity { SubNumber = 69 }
            };
            var sm = new BasicModel
            {
                subEntitySubNumber = 1
            };

            var maps = _matcher.Match(be.GetType().GetProperties(), sm.GetType().GetProperties());

            maps.Count(x => x.ToPropertyName == "subEntitySubNumber").ShouldEqual(1, "No Maps Produced");

            var map = maps.First(x => x.ToPropertyName == "subEntitySubNumber");

            map.FromComponents.Count().ShouldEqual(2);
            map.ToComponents.Count.ShouldEqual(1);

            map.FromComponents.First().Name.ShouldEqual("SubEntity");
            map.FromComponents.Skip(1).First().Name.ShouldEqual("SubNumber");
        }

        [Test]
        public void Should_Match_Flattened_Nullable()
        {
            var be = new BasicEntity { SubEntity = new SubEntity { Id = Guid.NewGuid() } };
            var sm = new BasicModel { subEntityId = Guid.NewGuid() };

            var maps = _matcher.Match(typeof(BasicEntity).GetProperties(), typeof(BasicModel).GetProperties());

            maps.Any(x => x.ToPropertyName == "subEntityId").ShouldBeTrue();
        }

        [Test]
        public void Should_Not_Match_Flattened_Of_Different_Type()
        {
            var be = new BasicEntity { };
            var sm = new BasicModel { };

            var maps = _matcher.Match(typeof(BasicEntity).GetProperties(), typeof(BasicModel).GetProperties());

            maps.Any(x => x.ToPropertyName == "subEntityAssplosion").ShouldBeFalse();
        }

        private Func<T, R> CreateLambda<T, R>(Func<Expression, Expression> accessor)
        {
            var param = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, R>>(accessor(param), param).Compile();
        }

        public class BasicEntity
        {
            public string Name { get; set; }
            public SubEntity SubEntity { get; set; }
        }

        public class SubEntity
        {
            public Guid Id { get; set; }
            public string SubName { get; set; }
            public string Assplosion { get; set; }
            public int SubNumber { get; set; }
        }

        public class BasicModel
        {
            public Guid? subEntityId { get; set; }
            public string name { get; set; }
            public string subEntitySubName { get; set; }
            public Guid? subEntityAssplosion { get; set; }
            public int? subEntitySubNumber { get; set; }
        }
    }
}

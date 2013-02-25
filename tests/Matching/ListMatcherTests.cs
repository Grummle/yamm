using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Should;
using yamm.Matching;

namespace tests.Matching
{
    [TestFixture]
    public class ListMatcherTests
    {
        private ListMatcher _matcher;

        [SetUp]
        public void Setup()
        {
            _matcher = new ListMatcher();
        }

        [Test]
        public void Should_Return_Map()
        {
            var maps = _matcher.Match(typeof(Entity).GetProperties(), typeof(Model).GetProperties());
            maps.Count(x => x.FromPropertyName == "SubEntities").ShouldEqual(1);
        }

        [Test]
        public void Should_Only_Return_Map_For_IList_With_Same_Name()
        {
            var maps = _matcher.Match(typeof(Entity).GetProperties(), typeof(Model).GetProperties());

            maps.All(map => map.ToComponents.Last().PropertyType.IsGenericType).ShouldBeTrue("Not Generic Type");
            maps.All(map => map.ToComponents.Last().PropertyType.GetGenericTypeDefinition() == typeof(IList<>)).ShouldBeTrue("Not IList");

            maps.All(map => map.FromComponents.Last().PropertyType.IsGenericType).ShouldBeTrue("Not Generic Type");
            maps.All(map => map.FromComponents.Last().PropertyType.GetGenericTypeDefinition() == typeof(IList<>)).ShouldBeTrue("Not IList");

            maps.All(map => map.FromComponents.Last().Name.ToLower() == map.ToComponents.Last().Name.ToLower()).ShouldBeTrue("Names don't match");
        }

        public class Entity
        {
            public IList<SubEntity> SubEntities { get; set; }
            public string Name { get; set; }
        }

        public class SubEntity
        {
            public string Name { get; set; }
        }

        public class Model
        {
            public IList<SubModel> subEntities { get; set; }
            public string name { get; set; }
        }

        public class SubModel
        {
            public string name { get; set; }
        }
    }


}

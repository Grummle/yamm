using System;
using System.Linq;
using NUnit.Framework;
using Should;
using yamm.Matching;

namespace tests.Matching
{
    [TestFixture]
    public class BasicMatcherTests
    {
        private BasicMatcher _matcher;
        private BasicEntity _be;
        private BasicModel _bm;

        [SetUp]
        public void Setup()
        {
            _matcher = new BasicMatcher();
            _be = new BasicEntity { Id = Guid.NewGuid(), Name = "Dillon", Number = 69 };
            _bm = new BasicModel { id = Guid.NewGuid(), name = "Dillon", number = 69 };
        }

        [Test]
        public void Should_Create_Expressions_For_Property_Access_String()
        {
            var maps = _matcher.Match(_be.GetType().GetProperties(), _bm.GetType().GetProperties());

            Helpers.CreateLambda<BasicEntity, string>(maps.First(x => x.FromPropertyName == "Name").AccessFromProperty)(_be).ShouldEqual(_be.Name);
            Helpers.CreateLambda<BasicModel, string>(maps.First(x => x.FromPropertyName == "Name").AccessToProperty)(_bm).ShouldEqual(_bm.name);
        }

        [Test]
        public void Should_Create_Expressions_For_Property_Access_Int()
        {
            var maps = _matcher.Match(_be.GetType().GetProperties(), _bm.GetType().GetProperties());

            Helpers.CreateLambda<BasicEntity, int>(maps.First(x => x.FromPropertyName == "Number").AccessFromProperty)(_be).ShouldEqual(_be.Number);
            Helpers.CreateLambda<BasicModel, int>(maps.First(x => x.FromPropertyName == "Number").AccessToProperty)(_bm).ShouldEqual(_bm.number);
        }

        [Test]
        public void Should_Create_Expressions_For_Property_Access_Guid()
        {
            var maps = _matcher.Match(_be.GetType().GetProperties(), _bm.GetType().GetProperties());

            Helpers.CreateLambda<BasicEntity, Guid>(maps.First(x => x.FromPropertyName == "Id").AccessFromProperty)(_be).ShouldEqual(_be.Id);
            Helpers.CreateLambda<BasicModel, Guid>(maps.First(x => x.FromPropertyName == "Id").AccessToProperty)(_bm).ShouldEqual(_bm.id);
        }



        public class BasicEntity
        {
            public string Name { get; set; }
            public Guid Id { get; set; }
            public int Number { get; set; }
        }

        public class BasicModel
        {
            public string name { get; set; }
            public Guid id { get; set; }
            public int number { get; set; }
        }
    }
}

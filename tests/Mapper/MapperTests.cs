using System;
using System.Collections.Generic;
using NUnit.Framework;
using Should;
using yamm;
using yamm.Matching;

namespace tests.Mapper
{
    [TestFixture]
    public class MapperTests
    {
        private yamm.Mapper.Mapper<BasicEntity, BasicModel> _mapper;
        private BasicEntity _be;
        private BasicModel _sm;

        [SetUp]
        public void Setup()
        {
            _mapper = new yamm.Mapper.Mapper<BasicEntity, BasicModel>(new List<IMatcher>
                                                             {
                                                                 new BasicMatcher(),
                                                                 new NullableMatcher(),
                                                                 new FlattenMatcher(),
                                                                 //new ListMatcher()
                                                             });
        }

        [Test]
        public void Should_Map_Simple_Fields()
        {
            _be = new BasicEntity { Name = "Michael", Id = Guid.NewGuid() };
            _sm = new BasicModel { name = "Not Michael", id = Guid.NewGuid() };

            Console.WriteLine("be.Name:{0} sm.name{1}", _be.Name, _sm.name);
            Console.WriteLine("be.Id:{0} sm.id{1}", _be.Id, _sm.id);

            _mapper.Map(_be, _sm);

            Console.WriteLine("be.Name:{0} sm.name{1}", _be.Name, _sm.name);
            Console.WriteLine("be.Id:{0} sm.id{1}", _be.Id, _sm.id);

            _sm.name.ShouldEqual(_be.Name);
            _sm.id.ShouldEqual(_be.Id);
        }

        [Test]
        public void Should_Map_Flattened_Fields()
        {
            _be = new BasicEntity
            {
                Name = "Michael",
                SubEntity = new SubEntity { SubName = "Mini Michael" },
                Id = Guid.NewGuid(),
                NullableInt = 69
            };
            _sm = new BasicModel
            {
                name = "Not Michael",
                subEntitySubName = "Not Mini Michael",
                nullableInt = 1
            };

            _mapper.Map(_be, _sm);

            _sm.name.ShouldEqual(_be.Name);
            _sm.id.ShouldEqual(_be.Id);
            _sm.subEntitySubName.ShouldEqual(_be.SubEntity.SubName);
            _sm.nullableInt.ShouldEqual(_be.NullableInt);
        }

        [Test]
        public void Should_Map_Nullable_Property()
        {
            _be = new BasicEntity { AnotherGuid = Guid.NewGuid() };
            _sm = new BasicModel { anotherGuid = Guid.NewGuid() };

            _mapper.Map(_be, _sm);

            _sm.anotherGuid.ShouldEqual(_be.AnotherGuid);
        }


        [Test]
        public void Should_Map_Flattened_Nullable_Property()
        {
            _be = new BasicEntity { SubEntity = new SubEntity { Id = Guid.NewGuid() } };
            _sm = new BasicModel { subEntityId = Guid.NewGuid() };

            _mapper.Map(_be, _sm);

            _sm.subEntityId.ShouldEqual(_be.SubEntity.Id);

        }

        [Test]
        public void Should_Not_Map_Property_When_Given_Model_Map()
        {
            _be = new BasicEntity { Name = "Michael", Id = Guid.NewGuid(), NotMappable = Guid.NewGuid() };
            _sm = new BasicModel { name = "Not Michael", id = Guid.NewGuid() };

            _mapper.ModelMap = new EntityMap();
            _mapper.Map(_be, _sm);

            _sm.notMappable.ShouldNotEqual(_be.NotMappable);
            _sm.name.ShouldEqual(_be.Name);
        }

        public class BasicEntity
        {
            public Guid Id { get; set; }
            public Guid AnotherGuid { get; set; }
            public string Name { get; set; }
            public SubEntity SubEntity { get; set; }
            public IList<SubEntity> SubEntities { get; set; }
            public int NullableInt { get; set; }
            public Guid NotMappable { get; set; }
        }

        public class SubEntity
        {
            public string SubName { get; set; }
            public Guid Id { get; set; }
        }

        public class BasicModel
        {
            public Guid id { get; set; }
            public Guid? anotherGuid { get; set; }
            public Guid? subEntityId { get; set; }
            public string name { get; set; }
            public string subEntitySubName { get; set; }
            public int? nullableInt { get; set; }
            public IList<SubModel> subEntities { get; set; }
            public Guid notMappable { get; set; }
        }

        public class SubModel
        {
            public string name { get; set; }
        }

        public class EntityMap : ModelMap<BasicEntity, BasicModel>
        {
            public EntityMap()
            {
                Property(x => x.name);
            }
        }
    }


}

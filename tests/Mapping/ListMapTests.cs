using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Should;
using yamm.Mapper;
using yamm.Mapping;

namespace tests.Mapping
{
    [TestFixture]
    public class ListMapTests
    {
        private ListMap _map;
        private PropertyInfo _fromInfo;
        private PropertyInfo _toInfo;
        private Entity _entity;
        private Model _model;

        [SetUp]
        public void Setup()
        {
            _fromInfo = typeof(Entity).GetProperty("SubEntities");
            _toInfo = typeof(Model).GetProperty("subEntities");

            _map = new ListMap();
            _map.ToComponents.Add(_toInfo);
            _map.FromComponents.Add(_fromInfo);

            _entity = new Entity
            {
                SubEntities = new List<SubEntity>
                                                {
                                                    new SubEntity
                                                        {
                                                            Name = "Dillon"
                                                        }
                                                }
            };

            _model = new Model
            {
                subEntities = new List<SubModel>
                                               {
                                                   new SubModel
                                                       {
                                                           name = "Not Dillon"
                                                       }
                                               }
            };
        }

        [Test]
        public void Should_Return_FromProperty_Name()
        {
            _map.FromPropertyName.ShouldEqual("SubEntities");
        }

        [Test]
        public void Should_Return_ToProperty_Name()
        {
            _map.ToPropertyName.ShouldEqual("subEntities");
        }

        [Test]
        public void Should_Create_Access_For_From()
        {
            Helpers.CreateLambda<Entity, IList<SubEntity>>(_map.AccessFromProperty)(_entity).ShouldEqual(_entity.SubEntities);
        }

        [Test]
        public void Should_Create_Access_For_To()
        {
            Helpers.CreateLambda<Model, IList<SubModel>>(_map.AccessToProperty)(_model).ShouldEqual(_model.subEntities);
        }

        [Test]
        public void Should_Create_Assign()
        {
            Helpers.ActionLambda<Entity, Model>(_map)(_entity, _model);
            _model.subEntities.First().name.ShouldEqual("Dillon");

            var mapper = new Mapper<Entity, Model>(null);
            Expression<Func<IList<Entity>, IList<Model>>> fark = e => e.Select(q => mapper.Map(q, new Model())).ToList();
        }

        public class Entity
        {
            public IList<SubEntity> SubEntities { get; set; }
        }

        public class SubEntity
        {
            public string Name { get; set; }
        }

        public class Model
        {
            public Model()
            {
                subEntities = new List<SubModel>();
            }

            public IList<SubModel> subEntities { get; set; }
        }

        public class SubModel
        {
            public string name { get; set; }
        }
    }
}

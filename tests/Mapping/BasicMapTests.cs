using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Should;
using yamm;
using yamm.Mapping;

namespace tests.Mapping
{
    [TestFixture]
    public class BasicMapTests
    {
        private BasicMap _map;
        private Entity _entity;
        private Model _model;
        
        [SetUp]
        public void Setup()
        {
            _map = new BasicMap();    
            _entity = new Entity
                          {
                              Name = "Entity",
                              SubEntity = new SubEntity
                                              {
                                                  Name = "SubEntity"
                                              }
                          };

            _model = new Model
                         {
                             name = "Model",
                             subEntityName = "subEntityName"
                         };
        }
        [Test]
        public void Shoul_Return_Dotted_ToPropertyName()
        {
            _map.ToComponents.Add(ForType<Model>.GetProperty(x=>x.name));
            _map.ToComponents.Add(ForType<Model>.GetProperty(x=>x.name));

            _map.ToPropertyName.ShouldEqual("name.name");
        }

        [Test]
        public void Shoul_Return_Dotted_FromPropertyName()
        {
            _map.FromComponents.Add(ForType<Entity>.GetProperty(x=>x.Name));
            _map.FromComponents.Add(ForType<Entity>.GetProperty(x=>x.Name));

            _map.FromPropertyName.ShouldEqual("Name.Name");
        }

        [Test]
        public void Should_Build_Property_Accessor_Expression()
        {
            _map.ToComponents.Add(ForType<Model>.GetProperty(x=>x.name));
            _map.FromComponents.Add(ForType<Entity>.GetProperty(x=>x.Name));

            Helpers.CreateLambda<Entity, string>(_map.AccessFromProperty)(_entity).ShouldEqual(_entity.Name);
            Helpers.CreateLambda<Model, string>(_map.AccessToProperty)(_model).ShouldEqual(_model.name);
        }

        [Test]
        public void Should_Build_Property_Accessor_Nested()
        {
            _map.ToComponents.Add(ForType<Model>.GetProperty(x=>x.subEntityName));
            _map.FromComponents.Add(ForType<Entity>.GetProperty(x=>x.SubEntity));
            _map.FromComponents.Add(ForType<SubEntity>.GetProperty(x=>x.Name));

            Helpers.CreateLambda<Entity, string>(_map.AccessFromProperty)(_entity).ShouldEqual(_entity.SubEntity.Name);
            Helpers.CreateLambda<Model, string>(_map.AccessToProperty)(_model).ShouldEqual(_model.subEntityName);
        }

        [Test]
        public void Should_Build_Property_Validator()
        {
            var map = new BasicMap();

            map.ToComponents.Add(typeof(Model).GetProperty("id"));
            map.FromComponents.Add(typeof(Entity).GetProperty("Id"));

            var validator = Helpers.CreateLambda<Entity, bool>(map.ValidateFrom);

            validator(_entity).ShouldBeTrue();
        }

        [Test]
        public void Should_Build_Property_Validator_For_Flatten()
        {
            var map = new BasicMap();

            map.ToComponents.Add(ForType<Model>.GetProperty(x=>x.subEntityName));
            map.FromComponents.Add(ForType<Entity>.GetProperty(x=>x.SubEntity));
            map.FromComponents.Add(ForType<SubEntity>.GetProperty(x=>x.Name));

            Helpers.CreateLambda<Entity, bool>(map.ValidateFrom)(_entity).ShouldBeTrue();
            _entity.SubEntity.Name = null;
            Helpers.CreateLambda<Entity, bool>(map.ValidateFrom)(_entity).ShouldBeFalse();
            _entity.SubEntity = null;
            Helpers.CreateLambda<Entity, bool>(map.ValidateFrom)(_entity).ShouldBeFalse();
        }


    }
}

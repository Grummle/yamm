using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Should;
using yamm;
using yamm.Mapping;

namespace tests.Mapping
{
    [TestFixture]
    public class MapTests
    {
        private Model _model;
        private Entity _entity;
        private Map _map;

        [SetUp]
        public void Setup()
        {
            _map = new Map();

            _entity = new Entity
            {
                Id = Guid.NewGuid(),
                Name = "Dillon",
                NullableGuid = Guid.NewGuid(),
                SubEntity = new SubEntity
                {
                    Id = Guid.NewGuid(),
                    NullableGuid = Guid.NewGuid(),
                    SubName = "SubDillon"
                }
            };

            _model = new Model
            {
                id = Guid.NewGuid(),
                name = "ModelDillon",
                nullableGuid = Guid.NewGuid(),
                subEntityId = Guid.NewGuid(),
                subEntityNullableGuid = Guid.NewGuid(),
                subEntitySubName = "ModelSubDillon"
            };
        }

        [Test]
        public void Shoul_Return_Dotted_ToPropertyName()
        {
            _map.ToComponents.Add(typeof(Model).GetProperty("name"));
            _map.ToComponents.Add(typeof(Model).GetProperty("name"));

            _map.ToPropertyName.ShouldEqual("name.name");
        }

        [Test]
        public void Shoul_Return_Dotted_FromPropertyName()
        {
            _map.FromComponents.Add(typeof(Model).GetProperty("name"));
            _map.FromComponents.Add(typeof(Model).GetProperty("name"));

            _map.FromPropertyName.ShouldEqual("name.name");
        }

        [Test]
        public void Should_Create_Basic_Property_Accesors()
        {
            _map.FromComponents.Add(typeof(Entity).GetProperty("Id"));
            _map.ToComponents.Add(typeof(Model).GetProperty("id"));

            CreateLambda<Entity, Guid>(_map.AccessFromProperty)(_entity).ShouldEqual(_entity.Id);
            CreateLambda<Model, Guid>(_map.AccessToProperty)(_model).ShouldEqual(_model.id);

            BuildAssign<Entity, Model>(_map)(_entity, _model);

            _model.id.ShouldEqual(_entity.Id);
        }

        [Test]
        public void Should_Create_Nested_Property_Accessors()
        {
            _map.FromComponents.Add(typeof(Entity).GetProperty("SubEntity"));
            _map.FromComponents.Add(typeof(SubEntity).GetProperty("Id"));
            _map.ToComponents.Add(typeof(Model).GetProperty("subEntityId"));

            CreateLambda<Entity, Guid>(_map.AccessFromProperty)(_entity).ShouldEqual(_entity.SubEntity.Id);
            CreateLambda<Model, Guid>(_map.AccessToProperty)(_model).ShouldEqual(_model.subEntityId);

            BuildAssign<Entity, Model>(_map)(_entity, _model);

            _model.subEntityId.ShouldEqual(_entity.SubEntity.Id);
        }

        [Test]
        public void Should_Create_Nullable_Property_Accessors()
        {
            _map.FromComponents.Add(typeof(Entity).GetProperty("NullableGuid"));
            _map.ToComponents.Add(typeof(Model).GetProperty("nullableGuid"));

            CreateLambda<Entity, Guid?>(_map.AccessFromProperty)(_entity).ShouldEqual(_entity.NullableGuid);
            CreateLambda<Model, Guid?>(_map.AccessToProperty)(_model).ShouldEqual(_model.nullableGuid);

            BuildAssign<Entity, Model>(_map)(_entity, _model);

            _model.nullableGuid.ShouldEqual(_entity.NullableGuid);
        }

        [Test]
        public void Should_Create_Flattened_Nullable_Property_Accessors()
        {
            _map.FromComponents.Add(typeof(Entity).GetProperty("SubEntity"));
            _map.FromComponents.Add(typeof(SubEntity).GetProperty("NullableGuid"));
            _map.ToComponents.Add(typeof(Model).GetProperty("subEntityNullableGuid"));

            CreateLambda<Entity, Guid?>(_map.AccessFromProperty)(_entity).ShouldEqual(_entity.SubEntity.NullableGuid);
            CreateLambda<Model, Guid?>(_map.AccessToProperty)(_model).ShouldEqual(_model.subEntityNullableGuid);

            BuildAssign<Entity, Model>(_map)(_entity, _model);

            _model.subEntityNullableGuid.ShouldEqual(_entity.SubEntity.NullableGuid);
        }

        [Test]
        public void Should_Build_Property_Validator_Basic_Property()
        {
            var entity = new Entity { Name = "Michael" };
            var entity2 = new Entity { };

            _map.FromComponents.Add(typeof(Entity).GetProperty("Name"));
            _map.ToComponents.Add(typeof(Model).GetProperty("name"));

            CreateLambda<Entity, bool>(_map.ValidateFrom)(entity).ShouldBeTrue();
            CreateLambda<Entity, bool>(_map.ValidateFrom)(entity2).ShouldBeFalse();
        }

        [Test]
        public void Should_Build_Property_Validator_Nullable_Property()
        {
            var entity = new Entity { NullableGuid = Guid.NewGuid() };
            var entity2 = new Entity { };

            _map.FromComponents.Add(typeof(Entity).GetProperty("NullableGuid"));
            _map.ToComponents.Add(typeof(Model).GetProperty("nullableGuid"));

            CreateLambda<Entity, bool>(_map.ValidateFrom)(entity).ShouldBeTrue();
            CreateLambda<Entity, bool>(_map.ValidateFrom)(entity2).ShouldBeTrue();
        }

        [Test]
        public void Should_Build_Property_Validator_Flattened_Property()
        {
            var entity = new Entity { SubEntity = new SubEntity { SubName = "SubDillon" } };
            var entity2 = new Entity { SubEntity = new SubEntity() };
            var entity3 = new Entity { };

            _map.FromComponents.Add(typeof(Entity).GetProperty("SubEntity"));
            _map.FromComponents.Add(typeof(SubEntity).GetProperty("SubName"));
            _map.ToComponents.Add(typeof(Model).GetProperty("subEntitySubName"));

            CreateLambda<Entity, bool>(_map.ValidateFrom)(entity).ShouldBeTrue();
            CreateLambda<Entity, bool>(_map.ValidateFrom)(entity2).ShouldBeFalse();
            CreateLambda<Entity, bool>(_map.ValidateFrom)(entity3).ShouldBeFalse();
        }

        [Test]
        public void Should_Build_Property_Validator_Nullable_Flattened_Property()
        {
            var entity = new Entity { SubEntity = new SubEntity { SubName = "SubDillon" } };
            var entity2 = new Entity { SubEntity = new SubEntity() };
            var entity3 = new Entity { };

            _map.FromComponents.Add(typeof(Entity).GetProperty("SubEntity"));
            _map.FromComponents.Add(typeof(SubEntity).GetProperty("SubName"));
            _map.ToComponents.Add(typeof(Model).GetProperty("subEntitySubName"));

            CreateLambda<Entity, bool>(_map.ValidateFrom)(entity).ShouldBeTrue();
            CreateLambda<Entity, bool>(_map.ValidateFrom)(entity2).ShouldBeFalse();
            CreateLambda<Entity, bool>(_map.ValidateFrom)(entity3).ShouldBeFalse();
        }

        [Test]
        public void asdf()
        {
            var type = typeof(Guid);
            //            var type = typeof (int);

            (type.IsValueType && !type.IsEnum && !type.IsPrimitive).WriteLine();
        }

        public Action<TEntity, TModel> BuildAssign<TEntity, TModel>(IMap map)
        {
            var e = Expression.Parameter(typeof(TEntity));
            var m = Expression.Parameter(typeof(TModel));

            var eProp = map.AccessFromProperty(e);
            var mProp = map.AccessToProperty(m);

            var assign = Expression.Assign(mProp, eProp);
            var lam = Expression.Lambda<Action<TEntity, TModel>>(assign, e, m);
            return lam.Compile();
        }

        private Func<T, R> CreateLambda<T, R>(Func<Expression, Expression> accessor)
        {

            var param = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, R>>(accessor(param), param).Compile();
        }

        public class Entity
        {
            public Guid Id { get; set; }
            public Guid NullableGuid { get; set; }
            public string Name { get; set; }
            public SubEntity SubEntity { get; set; }
            public int? NullableInt { get; set; }
        }

        public class SubEntity
        {
            public string SubName { get; set; }
            public Guid Id { get; set; }
            public Guid NullableGuid { get; set; }
        }

        public class Model
        {
            public Guid id { get; set; }
            public Guid? nullableGuid { get; set; }
            public string name { get; set; }
            public string subEntitySubName { get; set; }
            public Guid? subEntityNullableGuid { get; set; }
            public Guid subEntityId { get; set; }
            public int? nullableInt { get; set; }
        }
    }
}

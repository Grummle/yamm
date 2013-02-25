using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Should;
using yamm;
using yamm.Mapping;

namespace tests.Matching
{
    [TestFixture]
    public class NullableMapTests
    {
        [Test]
        public void Should_Build_Property_Accessor_Expression()
        {
            var be = new BasicEntity { Id = Guid.NewGuid() };
            var sm = new BasicModel { id = Guid.NewGuid() };
            var map = new NullableMap(be.GetType().GetProperty("Id"), sm.GetType().GetProperty("id"));

            CreateLambda<BasicEntity, Guid?>(map.AccessFromProperty)(be).ShouldEqual(be.Id);
            CreateLambda<BasicModel, Guid?>(map.AccessToProperty)(sm).ShouldEqual(sm.id.Value);

            BuildAssign<BasicEntity, BasicModel>(map)(be, sm);

            sm.id.ShouldEqual(be.Id);
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

        [Test]
        public void Should_Build_Property_Accessor_Nested_Expression()
        {
            var be = new BasicEntity { SubEntity = new SubEntity { Id = Guid.NewGuid() } };
            var sm = new BasicModel { subEntityId = Guid.NewGuid() };
            var map = new NullableMap();

            map.ToComponents.Add(typeof(BasicModel).GetProperty("subEntityId"));
            map.FromComponents.Add(typeof(BasicEntity).GetProperty("SubEntity"));
            map.FromComponents.Add(typeof(SubEntity).GetProperty("Id"));

            CreateLambda<BasicEntity, Guid?>(map.AccessFromProperty)(be).ShouldEqual(be.SubEntity.Id);
            CreateLambda<BasicModel, Guid?>(map.AccessToProperty)(sm).ShouldEqual(sm.subEntityId);

        }

        [Test]
        public void Should_Build_Property_Validator()
        {
            var be = new BasicEntity();
            var map = new NullableMap();

            map.ToComponents.Add(typeof(BasicModel).GetProperty("id"));
            map.FromComponents.Add(typeof(BasicEntity).GetProperty("Id"));

            CreateLambda<BasicEntity, bool>(map.ValidateFrom)(be).ShouldBeTrue();
        }

        //[Test]
        //public void Should_Build_Property_Validator_For_Flatten()
        //{
        //    var be = new BasicEntity { SubEntity = new SubEntity { Id=Guid.NewGuid() } };
        //    var be2 = new BasicEntity { SubEntity = new SubEntity() };
        //    var be3 = new BasicEntity ();
        //    var map = new NullableMap();

        //    map.ToInfo.Add(typeof(BasicModel).GetProperty("subEntityId"));
        //    map.FromInfo.Add(typeof(BasicEntity).GetProperty("SubEntity"));
        //    map.FromInfo.Add(typeof(SubEntity).GetProperty("Id"));

        //    CreateLambda<BasicEntity, bool>(map.ValidateFrom)(be).ShouldBeTrue();
        //    CreateLambda<BasicEntity, bool>(map.ValidateFrom)(be2).ShouldBeFalse();
        //    CreateLambda<BasicEntity, bool>(map.ValidateFrom)(be3).ShouldBeFalse();
        //}

        private Func<T, R> CreateLambda<T, R>(Func<Expression, Expression> accessor)
        {

            var param = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, R>>(accessor(param), param).Compile();
        }

        public class BasicEntity
        {
            public Guid Id { get; set; }
            public SubEntity SubEntity { get; set; }
        }

        public class SubEntity
        {
            public Guid Id { get; set; }
        }

        public class BasicModel
        {
            public Guid? id { get; set; }
            public Guid? subEntityId { get; set; }
        }
    }
}

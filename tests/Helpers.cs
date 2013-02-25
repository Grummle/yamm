using System;
using System.Linq.Expressions;
using yamm;
using yamm.Mapping;

namespace tests
{
    public static class Helpers
    {
        public static Func<T, R> CreateLambda<T, R>(Func<Expression, Expression> accessor)
        {
            var param = Expression.Parameter(typeof(T));
            return Expression.Lambda<Func<T, R>>(accessor(param), param).Compile();
        }

        public static Action<E, M> ActionLambda<E, M>(IMap map)
        {
            var param = Expression.Parameter(typeof(E));
            var param2 = Expression.Parameter(typeof(M));
            return Expression.Lambda<Action<E, M>>(map.Assign(param2, param), param, param2).Compile();
        }

        public static Action<TEntity, TModel> BuildAssign<TEntity, TModel>(IMap map)
        {
            var e = Expression.Parameter(typeof(TEntity));
            var m = Expression.Parameter(typeof(TModel));

            var eProp = map.AccessFromProperty(e);
            var mProp = map.AccessToProperty(m);

            var assign = Expression.Assign(mProp, eProp);
            var lam = Expression.Lambda<Action<TEntity, TModel>>(assign, e, m);
            return lam.Compile();
        }
    }
}

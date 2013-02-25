using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using yamm.Mapping;
using yamm.Matching;

namespace yamm.Mapper
{
    public class Mapper<TFrom, TTo>
    {
        private readonly IEnumerable<IMatcher> _matchers;
        private IList<IMap> _maps = new List<IMap>();
        public ModelMap<TFrom, TTo> ModelMap { get; set; }

        public Mapper(IEnumerable<IMatcher> matchers)
        {
            _matchers = matchers;
        }

        public TTo Map(TFrom from, TTo to)
        {
            var fromProperties = typeof(TFrom).GetProperties();
            var toProperties = typeof(TTo).GetProperties();

            foreach (var matcher in _matchers)
            {
                _maps.AddRange(matcher.Match(fromProperties, toProperties));
            }

            var fromParamater = Expression.Parameter(typeof(TFrom));
            var toParamater = Expression.Parameter(typeof(TTo));


            if (ModelMap.IsNotNull())
                _maps = _maps.Where(map => ModelMap.Properties.Select(x => x.Name).Contains(map.ToPropertyName)).ToList();

            var assignments = _maps.Where(x => Validate(from, x))
                                   .Select(x => x.Assign(toParamater, fromParamater));

            var block = Expression.Block(assignments);

            var lam = Expression.Lambda<Action<TFrom, TTo>>(block, fromParamater, toParamater);

            var func = lam.Compile();

            func(from, to);

            return to;
        }

        public bool Validate(TFrom from, IMap map)
        {
            var param = Expression.Parameter(typeof(TFrom));
            var del = Expression.Lambda<Func<TFrom, bool>>(map.ValidateFrom(param), param).Compile();
            return del(from);
        }
    }
}

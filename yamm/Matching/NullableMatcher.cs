using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using yamm.Mapping;

namespace yamm.Matching
{
    public class NullableMatcher : IMatcher
    {
        private IList<PropertyInfo> _fromProperties;
        private IList<PropertyInfo> _toProperties;
        private IList<IMap> _maps = new List<IMap>();

        public IList<IMap> Match(IEnumerable<PropertyInfo> fromProperties, IEnumerable<PropertyInfo> toProperties)
        {
            _fromProperties = fromProperties.ToList();
            _toProperties = toProperties.ToList();

            MatchProperties();
            return _maps;
        }

        private void MatchProperties()
        {
            foreach (var fromProp in _fromProperties)
            {
                var matches = _toProperties.Where(x => x.Name.ToLower() == fromProp.Name.ToLower())
                                           .Where(x =>
                                                          ((x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                                                    && x.PropertyType.GetGenericArguments()[0] == fromProp.PropertyType)
                                                      );

                if (!matches.Any()) continue;

                _maps.Add(new NullableMap(fromProp, matches.First()));
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using yamm.Mapping;

namespace yamm.Matching
{
    public class ListMatcher : IMatcher
    {
        private IList<IMap> _maps = new List<IMap>();

        public IList<IMap> Match(IEnumerable<PropertyInfo> fromProperties, IEnumerable<PropertyInfo> toProperties)
        {
            foreach (var from in fromProperties.Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                _maps.AddRange(toProperties.Where(to => to.Name.ToLower() == from.Name.ToLower()).Select(to => new ListMap(from, to)));
            }

            return _maps;
        }
    }
}

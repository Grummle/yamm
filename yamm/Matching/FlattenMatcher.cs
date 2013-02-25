using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using yamm.Mapping;

namespace yamm.Matching
{
    public class FlattenMatcher : IMatcher
    {
        private readonly IEnumerable<IMatcher> _matchers;

        public IList<IMap> Match(IEnumerable<PropertyInfo> fromProperties, IEnumerable<PropertyInfo> toProperties)
        {
            return toProperties.Select(x => MatchProperty(new DeconstructedProperty(x, x.Name.Deconstruct()), fromProperties))
                            .Where(x => x.IsNotNull())
                            .Where(x => x.FromComponents.Count > 1)
                            .ToList();
        }

        private IMap MatchProperty(DeconstructedProperty to, IEnumerable<PropertyInfo> fromProperties, IMap map = null)
        {
            var index = 0;
            var current = "";
            foreach (var token in to.RemainingNameTokens)
            {
                current += token;
                index += 1;
                var match = fromProperties.FirstOrDefault(x => x.Name.ToLower() == current.ToLower());
                if (match.IsNotNull())
                {
                    if (map.IsNull())
                        map = new Map(match, to.OriginalPropertyInfo);
                    else
                    {
                        if (to.RemainingNameTokens.Any())
                            map.FromComponents.Add(match);

                    }

                    to.RemainingNameTokens = to.RemainingNameTokens.Skip(index).ToArray();
                    map = MatchProperty(to, match.PropertyType.GetProperties(), map);
                }
            }

            if (map.IsNotNull() && map.FromComponents.Last().PropertyType != map.ToComponents.Last().PropertyType
                &&
                !((map.ToComponents.Last().PropertyType.IsGenericType &&
                  map.ToComponents.Last().PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                 &&
                 map.ToComponents.Last().PropertyType.GetGenericArguments()[0] == map.FromComponents.Last().PropertyType))
            {
                return null;
            }


            return map;
        }


        public class DeconstructedProperty
        {
            public PropertyInfo OriginalPropertyInfo { get; set; }
            public string[] RemainingNameTokens { get; set; }

            public DeconstructedProperty(PropertyInfo originalPropertyInfo, string[] RemainingNameTokens)
            {
                OriginalPropertyInfo = originalPropertyInfo;
                this.RemainingNameTokens = RemainingNameTokens;
            }
        }

    }
}

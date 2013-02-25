using System.Collections.Generic;
using System.Reflection;
using yamm.Mapping;

namespace yamm.Matching
{
    public interface IMatcher
    {
        IList<IMap> Match(IEnumerable<PropertyInfo> fromProperties, IEnumerable<PropertyInfo> toProperties);
    }
}
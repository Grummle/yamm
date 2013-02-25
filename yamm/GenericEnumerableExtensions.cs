using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yamm
{
    public static class GenericEnumerableExtensions
    {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T> eachAction)
        {
            foreach (T obj in values)
                eachAction(obj);
            return values;
        }

        public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            Each<T>(items, new Action<T>(((ICollection<T>)list).Add));
            return list;
        }
    }
}

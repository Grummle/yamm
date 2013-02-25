using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace yamm
{
    public static class PropertyInfoExtensions
    {
        public static bool IsNullableType(this PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.IsNullableType();
        }
    }
}

using System;

namespace yamm
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object objyn)
        {
            return objyn == null;
        }

        public static bool IsNotNull(this object objyn)
        {
            return objyn != null;
        }

        public static void WriteLine(this object value, params object[] stuff)
        {
            Console.WriteLine(value.ToString(), stuff);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CryptoDoge.DAL.UnitTests
{
    internal static class EqualityComparerHelper
    {
        public static bool SequenceEqual<T>(this IList<T> x, IList<T> y, Func<T, T, bool> comparer)
        {
            if (x == null || y == null)
                return x == y;

            if (x.Count != y.Count)
                return false;

            for (int i = 0; i < x.Count; i++)
            {
                var xItem = x[i];
                var yItem = y[i];
                if (!comparer(xItem, yItem))
                    return false;
            }
            return true;
        }

        public static bool PropertiesEqual<T>(T x, T y, params string[] ignore) where T : class
        {
            if (x == null || y == null)
                return x == y;

            Type type = typeof(T);
            List<string> ignoreList = new List<string>(ignore);
            foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (ignoreList.Contains(pi.Name))
                    continue;

                var propertyType = type.GetProperty(pi.Name).PropertyType;
                object xValue = type.GetProperty(pi.Name).GetValue(x, null);
                object yValue = type.GetProperty(pi.Name).GetValue(y, null);

                // Compare lists
                if (propertyType.IsAssignableTo(typeof(IEnumerable<object>)))
                {
                    if (xValue != yValue && (xValue == null || yValue == null))
                        return false;

                    if (!(xValue as IEnumerable<object>).SequenceEqual(yValue as IEnumerable<object>))
                        return false;
                }
                else if (xValue != yValue && (xValue == null || !xValue.Equals(yValue)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

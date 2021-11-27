﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CryptoDoge.Shared
{
    public static class EqualityComparerHelper
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
            var ignoreList = new List<string>(ignore);

            foreach (string name in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.Name))
            {
                if (ignoreList.Contains(name))
                    continue;

                var propertyType = type.GetProperty(name).PropertyType;
                object xValue = type.GetProperty(name).GetValue(x, null);
                object yValue = type.GetProperty(name).GetValue(y, null);

                return CompareLists(propertyType, xValue, yValue);
            }
            return true;
        }

        private static bool CompareLists(Type propertyType, object xValue, object yValue)
        {
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

            return true;
        }
    }
}

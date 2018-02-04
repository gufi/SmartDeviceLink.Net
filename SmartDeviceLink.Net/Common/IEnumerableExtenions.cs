using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDeviceLink.Net.Common
{
    public static class IEnumerableExtenions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> enumerable, int inGroupsOf)
        {
            var listEnumerable = (enumerable is List<T> list) ? list : enumerable.ToList();
            for (int i = 0; i < listEnumerable.Count(); i += inGroupsOf)
            {
                yield return listEnumerable.GetRange(i, Math.Min(inGroupsOf, listEnumerable.Count() - i));
            }
        }
    }
}

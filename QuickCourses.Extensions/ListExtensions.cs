using System.Collections.Generic;

namespace QuickCourses.Extensions
{
    public static class ListExtensions
    {
        public static bool TryGetValue<TValue>(this List<TValue> list, int index, out TValue result)
        {
            if (list.Count < index)
            {
                result = default(TValue);
                return false;
            }

            result = list[index];
            return true;
        }
    }
}
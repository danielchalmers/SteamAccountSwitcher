using System.Collections.Generic;

namespace SteamAccountSwitcher
{
    public static class ListUtilities
    {
        private static T Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            var tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list[indexB];
        }

        public static T MoveToTop<T>(this IList<T> list, T item)
        {
            list.Remove(item);
            list.Insert(0, item);
            return item;
        }

        public static T MoveToBottom<T>(this IList<T> list, T item)
        {
            list.Remove(item);
            list.Insert(list.Count, item);
            return item;
        }

        public static T MoveUp<T>(this IList<T> list, T item)
        {
            var index = list.IndexOf(item);
            if (index == 0)
            {
                return list.MoveToBottom(item);
            }
            return list.Swap(index, index - 1);
        }

        public static T MoveDown<T>(this IList<T> list, T item)
        {
            var index = list.IndexOf(item);
            if (index + 1 > list.Count - 1)
            {
                return list.MoveToTop(item);
            }
            return list.Swap(index, index + 1);
        }
    }
}
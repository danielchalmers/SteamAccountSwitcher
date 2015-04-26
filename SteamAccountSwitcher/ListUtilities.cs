#region

using System.Collections.Generic;

#endregion

namespace SteamAccountSwitcher
{
	public static class ListUtilities
	{
		public static void Swap<T>(this IList<T> list, int indexA, int indexB)
		{
			var tmp = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = tmp;
		}
	}
}
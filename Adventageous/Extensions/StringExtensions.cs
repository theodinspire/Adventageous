using System.Runtime.CompilerServices;

namespace Adventageous.Extensions;

public static class StringExtensions
{
	public static T? From<T>(this string self, Parser<T> parser)
		where T : struct
	{
		if (string.IsNullOrEmpty(self))
			return null;
		if (parser(self, out var result))
			return result;
		return null;
	}

	public delegate bool Parser<T>(string value, out T result);

	public static IEnumerable<int> AllIndicesOfAny(this string self, char[] characters)
	{
		var index = 0;
		do
		{
			var next = self.IndexOfAny(characters, index);
			if (next >= 0)
				yield return next;
			index = next + 1;
		}
		while (index > 0);
	}
}
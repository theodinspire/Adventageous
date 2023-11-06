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
}
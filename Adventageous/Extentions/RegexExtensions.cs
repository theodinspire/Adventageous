using System.Text.RegularExpressions;

namespace Adventageous.Extensions;

public static class RegexExtensions
{
	public static bool TryMatch(this Regex expression, string s, out Match match)
	{
		match = expression.Match(s);
		return match.Success;
	}
}
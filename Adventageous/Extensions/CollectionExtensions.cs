namespace Adventageous.Extensions;

public static class CollectionExtensions
{
	public static bool ContainsAny<T>(this IReadOnlyCollection<T> self, params T[] items) =>
		items.Any(self.Contains);
}
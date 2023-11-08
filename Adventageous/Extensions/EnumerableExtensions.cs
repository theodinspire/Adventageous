using Adventageous.Enums;

namespace Adventageous.Extensions;

public static class EnumerableExtensions
{
	#region Group

	public static IEnumerable<IEnumerable<T>> GroupInto<T>(this IEnumerable<T> collection, int count)
	{
		using var enumerator = collection.GetEnumerator();
		while (enumerator.MoveNext())
		{
			yield return GroupIntoHelper(enumerator, count);
		}
	}

	private static IEnumerable<T> GroupIntoHelper<T>(IEnumerator<T> enumerator, int count)
	{
		do
		{
			yield return enumerator.Current;
			count--;
		}
		while ( count > 0 && enumerator.MoveNext());
	}

	#endregion

	#region SelectNotNull

	public static IEnumerable<T> SelectNotNull<T>(this IEnumerable<T?> self)
		where T : struct =>
		self.Where(x => x.HasValue).Select(x => x!.Value);

	public static IEnumerable<T> SelectNotNull<T>(this IEnumerable<T?> self)
		where T : class =>
		self.Where(x => x != null).Cast<T>();

	public static IEnumerable<TResult> SelectNotNull<TSource, TResult>(this IEnumerable<TSource?> self, Func<TSource?, TResult?> selector)
		where TSource : struct
		where TResult : struct =>
		self.Select(selector).SelectNotNull();

	public static IEnumerable<TResult> SelectNotNull<TSource, TResult>(this IEnumerable<TSource?> self, Func<TSource?, int, TResult?> selector)
		where TSource : struct
		where TResult : struct =>
		self.Select(selector).SelectNotNull();

	public static IEnumerable<TResult> SelectNotNull<TSource, TResult>(this IEnumerable<TSource> self, Func<TSource, TResult?> selector)
		where TSource : struct
		where TResult : struct =>
		self.Select(selector).SelectNotNull();

	#endregion

	#region Indexed

	public static IEnumerable<(T Item, int Index)> Indexed<T>(this IEnumerable<T> self, int start = 0) =>
		self.Select((x, i) => (Item: x, Index: i + start));

	#endregion
}
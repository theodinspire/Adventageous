namespace Adventageous.Extensions;

public static class EnumerableExtensions
{
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
}
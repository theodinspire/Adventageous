namespace Adventageous.Extensions;

public static class StackExtensions
{
	public static IEnumerable<T> PopMany<T>(this Stack<T> self, int count)
	{
		return Enumerable.Range(1, count).TakeWhile(_ => self.Count != 0).Select(_ => self.Pop());
	}

	public static void PushMany<T>(this Stack<T> self, IEnumerable<T> items)
	{
		foreach (var item in items)
		{
			self.Push(item);
		}
	}
}
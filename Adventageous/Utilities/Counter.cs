namespace Adventageous.Utilities;

public class Counter<T>
	where T: IEquatable<T>
{
	private readonly Dictionary<T, int> container;

	public Counter()
	{
		this.container = new Dictionary<T, int>();
	}

	public Counter(IEnumerable<T> items)
	{
		this.container = new Dictionary<T, int>();
		foreach (var item in items)
		{
			if (this.container.ContainsKey(item))
				this.container[item] = this.container[item] += 1;
			else
				this.container.Add(item, 1);
		}
	}

	public int this[T item]
	{
		get => this.container.TryGetValue(item, out var count) ? count : 0;
		private set => this.container[item] = value;
	}

	public IEnumerable<T> Items => this.container.Where(p => p.Value != 0).Select(p => p.Key);

	/// <summary>
	/// Adds a count of the item to the counter.
	/// </summary>
	/// <param name="item">The item to be counted.</param>
	/// <returns>The new count of the item.</returns>
	public int Count(T item)
	{
		return ++this[item];
	}
}
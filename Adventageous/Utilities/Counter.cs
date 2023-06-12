using System.Net.Sockets;

namespace Adventageous.Utilities;

public class Counter<T>
	where T : notnull
{
	private readonly Dictionary<T, int> counts;

	public Counter()
	{
		this.counts = new Dictionary<T, int>();
	}

	public Counter(IEnumerable<T> collection)
	{
		this.counts = new Dictionary<T, int>();
		
		foreach (var item in collection)
		{
			this.Add(item);
		}
	}

	public IEnumerable<T> Items => this.counts.Where(pair => pair.Value > 0).Select(pair => pair.Key);

	public int GetCount(T item)
	{
		return this.counts.TryGetValue(item, out var c) ? c : 0;
	}

	public int Add(T item, int count = 1)
	{
		var total = this.GetCount(item) + count;
		this.counts[item] = total;
		return total;
	}

	public Dictionary<T, int> GetAllCounts() => this.counts.ToDictionary(pair => pair.Key, pair => pair.Value);
}
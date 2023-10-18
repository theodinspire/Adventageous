using System.Collections;

namespace Adventageous.DataStructure;

public class Grid<T> : IEnumerable<KeyValuePair<Point, T>>
{
	private Dictionary<Point, T> data = new Dictionary<Point, T>();

	public Grid()
	{
	}

	public T? this[Point point]
	{
		get => this.data.TryGetValue(point, out var value) ? value : default(T?);
		set
		{
			if (value is null)
				this.data.Remove(point);
			else
				this.data[point] = value;
		}
	}

	public T Get(Point point, T defaultValue) =>
		this.TryGetValue(point, out var t) ? t : defaultValue;

	public bool TryGetValue(Point point, out T t) =>
		this.data.TryGetValue(point, out t);

	public ICollection<Point> Keys => this.data.Keys;

	public void Clear() => this.data.Clear();

	public IEnumerator<KeyValuePair<Point, T>> GetEnumerator() => this.data.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
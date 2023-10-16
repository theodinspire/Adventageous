#nullable enable
namespace Adventageous.DataModel;

public class Grid<T>
{
	private Dictionary<Point, T> data = new Dictionary<Point, T>();

	public Grid()
	{
	}

	public T? this[Point point]
	{
		get => this.data.TryGetValue(point, out var value) ? value : default(T?);
		set { if (value is not null) this.data.Add(point, value); }
	}

	public ICollection<Point> Mapped => this.data.Keys;

	public void Clear() => this.data.Clear();
}
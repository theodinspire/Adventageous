using System.Runtime;
using System.Text;
using Adventageous.DataStructure;

namespace Adventageous.Days;

public class Day14
{
	private static readonly Point Origin = (500, 0);

	private const char Rock = '#';
	private const char Sand = 'o';

	private Grid<char> map = new Grid<char>();

	private int left   = Origin.X;
	private int right  = Origin.X;
	private int top    = Origin.Y;
	private int bottom = Origin.Y;

	public Day14(Stream input)
	{
		using var reader = new StreamReader(input);

		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is null)
				continue;

			var scan = Parse(line).ToArray();

			this.Process(scan);
		}
	}

	public int First()
	{
		this.Fill();

		return this.GetSandVolume();
	}

	public int Second()
	{
		var newBottom = this.bottom - 2;
		var l = new Point(Origin.X + newBottom - 10, newBottom);
		var r = new Point(Origin.X - newBottom + 10, newBottom);

		this.Process(new[] { l, r });
		this.Fill();
		return this.GetSandVolume();
	}

	private static IEnumerable<Point> Parse(string line)
	{
		return line.Split(" -> ").Select(Point.Parse).Select(p => p.Transformed((x, y) => (x, -y)));
	}

	private void Process(Point[] scan)
	{
		var current = scan.FirstOrDefault();
		foreach (var next in scan[1..])
		{
			this.left = int.Min(this.left, current.X);
			this.right = int.Max(this.right, current.X);
			this.top = int.Max(this.top, current.Y);
			this.bottom = int.Min(this.bottom, current.Y);

			if (current == next) {}
			else if (current.X == next.X)
			{
				foreach (var y in Interval.HalfOpen(current.Y, next.Y))
				{
					this.map[(current.X, y)] = Rock;
				}
			}
			else if (current.Y == next.Y)
			{
				foreach (var x in Interval.HalfOpen(current.X, next.X))
				{
					this.map[(x, current.Y)] = Rock;
				}
			}

			current = next;
		}

		// Update for last point
		this.left = int.Min(this.left, current.X);
		this.right = int.Max(this.right, current.X);
		this.top = int.Max(this.top, current.Y);
		this.bottom = int.Min(this.bottom, current.Y);

		this.map[current] = Rock;
	}

	private void Fill()
	{
		var stack = new Stack<Point>(new[] { Origin });

		while (stack.TryPeek(out var point))
		{
			if (point.Y <= this.bottom)
				break;

			if (!this.map.Keys.Contains(point.Down))
			{
				stack.Push(point.Down);
				continue;
			}

			if (!this.map.Keys.Contains(point.DownLeft))
			{
				stack.Push(point.DownLeft);
				continue;
			}

			if (!this.map.Keys.Contains(point.DownRight))
			{
				stack.Push(point.DownRight);
				continue;
			}

			this.map[stack.Pop()] = Sand;
		}
	}

	private int GetSandVolume()
	{
		return this.map.Count(x => x.Value == Sand);
	}

	private void Print()
	{
		var v = Interval.Closed(this.top, this.bottom);
		var h = Interval.Closed(this.left, this.right);

		var sb = new StringBuilder(v.Count * (h.Count + 2));

		foreach (var y in v)
		{
			foreach (var x in h)
			{
				sb.Append(this.map.Get((x, y), '.'));
			}

			sb.AppendLine();
		}

		Console.WriteLine(sb.ToString());
	}
}
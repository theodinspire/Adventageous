using Adventageous.DataModel;

namespace Adventageous.Days;

public class Day12
{
	private Grid<int> Elevation;

	private Grid<int> Cost = new Grid<int>();

	private Point Start;

	private Point Target;

	private Queue<Point> Queue = new Queue<Point>();

	public Day12(Stream input)
	{
		this.Elevation =new Grid<int>();

		var row = 0;

		using var reader = new StreamReader(input);

		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is null) break;

			for (var i = 0; i < line.Length; ++i)
			{
				var point = new Point(row, i);
				var character = line[i];

				if (character == 'S')
				{
					this.Start = point;
					this.Elevation[point] = 0;

					continue;
				}

				if (character == 'E')
				{
					this.Target = point;
					this.Elevation[point] = 25;

					continue;
				}

				this.Elevation[point] = (int)character - 97;
			}

			++row;
		}

	}

	public int First()
	{
		this.Cost[this.Start] = 0;
		this.Queue.Enqueue(this.Start);

		while (this.Queue.TryDequeue(out var current))
		{
			var elevation = this.Elevation[current];
			var cost = this.Cost[current] + 1;
			var next = current.TaxiCabNeighbors
				.Where(x => !this.Cost.Mapped.Contains(x))
				.Where(x => this.Elevation.Mapped.Contains(x))
				.Where(x => this.Elevation[x] <= elevation + 1)
				.ToHashSet();

			if (next.Contains(this.Target))
				return cost;

			foreach (var point in next)
			{
				this.Cost[point] = cost;
				this.Queue.Enqueue(point);
			}
		}

		return int.MinValue;
	}

	public int Second()
	{
		this.Cost.Clear();
		this.Queue.Clear();

		this.Cost[this.Target] = 0; // Ensures that cost at this.Target is 0 when retur
		this.Queue.Enqueue(this.Target);

		while (this.Queue.TryDequeue(out var current))
		{
			var elevation = this.Elevation[current];
			var cost = this.Cost[current];

			if (elevation == 0) return cost;

			var next = current.TaxiCabNeighbors
				.Where(x => !this.Cost.Mapped.Contains(x))
				.Where(x => this.Elevation.Mapped.Contains(x))
				.Where(x => this.Elevation[x] >= elevation - 1)
				.ToHashSet();

			foreach (var point in next)
			{
				this.Cost[point] = cost + 1;
				this.Queue.Enqueue(point);
			}
		}

		return int.MinValue;
	}
}
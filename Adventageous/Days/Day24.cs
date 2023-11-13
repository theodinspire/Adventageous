using System.Text;
using Adventageous.DataStructure;
using Adventageous.Enums;
using Adventageous.Extensions;

namespace Adventageous.Days;

public class Day24
{
	private readonly Point start;
	private readonly Point end;

	private readonly int maxX;
	private readonly int maxY;

	private readonly List<Blizzard> blizzards = new List<Blizzard>();
	private int iteration = 0;

	public Day24(Stream input)
	{
		using var reader = new StreamReader(input);
		var lines = reader.Lines().ToList();

		var first = lines.First();
		this.maxX = first.Length - 1;
		this.maxY = lines.Count - 1;

		this.start = new Point(first.IndexOf('.'), this.maxY);
		this.end = new Point(lines.Last().IndexOf('.'), 0);

		for (var y = 1; y < this.maxY; ++y)
		{
			for (var x = 1; x < this.maxX; ++x)
			{
				var c = lines[this.maxY - y][x];
				Direction? direction = c switch
				{
					'>' => Direction.Right,
					'v' => Direction.Down,
					'<' => Direction.Left,
					'^' => Direction.Up,
					_ => null,
				};

				if (!direction.HasValue)
					continue;

				this.blizzards.Add(new Blizzard((x, y), direction.Value));
			}
		}
	}

	public int First()
	{
		this.Travel(this.start, this.end);

		return this.iteration;
	}

	public int Second()
	{
		this.Travel(this.end, this.start);
		this.Travel(this.start, this.end);

		return this.iteration;
	}

	private void Travel(Point beginning, Point ending)
	{
		var locations = new HashSet<Point> { beginning };

		while (!locations.Contains(ending) && locations.Count > 0)
		{
			++this.iteration;

			foreach (var blizzard in this.blizzards)
				this.Move(blizzard);

			var nextBlizzardLocations = this.blizzards.Select(x => x.Location).ToHashSet();
			locations = locations
				.SelectMany(x => x.TaxiCabNeighborsAndSelf)
				.Where(this.Filter)
				.ToHashSet()
				.Except(nextBlizzardLocations)
				.ToHashSet();
		}
	}

	private void Print()
	{
		var builder = new StringBuilder();

		for (var x = 0; x <= this.maxX; ++x)
			builder.Append(x == this.start.X ? '.' : '#');
		builder.AppendLine();

		for (var y = this.maxY - 1; y > 0; --y)
		{
			builder.Append('#');
			for (var x = 1; x < this.maxX; ++x)
			{
				var local = this.blizzards.Where(b => b.Location == (x, y)).ToList();

				switch (local.Count)
				{
					case <= 0:
						builder.Append('.');
						break;
					case >= 10:
						builder.Append('!');
						break;
					case > 1:
						builder.Append(local.Count);
						break;
					default:
						builder.Append(local.Single().Character);
						break;
				}
			}

			builder.AppendLine("#");
		}

		for (var x = 0; x <= this.maxX; ++x)
			builder.Append(x == this.end.X ? '.' : '#');

		Console.WriteLine(builder.ToString());
		Console.WriteLine();
	}

	private void Move(Blizzard blizzard)
	{
		var (x, y) = blizzard.Target;

		if (x == 0)
			x = this.maxX - 1;
		else if (x == this.maxX)
			x = 1;
		else if (y == 0)
			y = this.maxY - 1;
		else if (y == this.maxY)
			y = 1;

		blizzard.Move((x, y));
	}

	private bool Filter(Point point)
	{
		if (point == this.start)
			return true;
		if (point == this.end)
			return true;

		return (point.X > 0 && point.X < this.maxX && point.Y > 0 && point.Y < this.maxY);
	}

	private class Blizzard
	{
		private readonly Direction direction;

		public Blizzard(Point location, Direction direction)
		{
			this.Location = location;
			this.direction = direction;
		}

		public Point Location { get; private set; }

		public Point Target => this.Location[this.direction];

		public void Move(Point point)
		{
			this.Location = point;
		}

		public Blizzard Clone() => new Blizzard(this.Location, this.direction);

		public char Character => this.direction switch
		{
			Direction.Up => '^',
			Direction.Down => 'v',
			Direction.Left => '<',
			Direction.Right => '>',
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}
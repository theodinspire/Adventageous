using System.Text;
using Adventageous.DataStructure;
using Adventageous.Enums;
using Adventageous.Extensions;
using Adventageous.Utilities;

namespace Adventageous.Days;

public class Day23
{
	private readonly IReadOnlySet<Point> initialPositions;

	public Day23(Stream input)
	{
		using var reader = new StreamReader(input);
		var positions = new HashSet<Point>();

		foreach (var (line, y) in reader.Lines().Reverse().Indexed(1))
		{
			foreach (var (character, x) in line.Indexed(1))
			{
				if (character != '#') continue;
				positions.Add((x, y));
			}
		}

		this.initialPositions = positions;
	}

	public int First()
	{
		var elves = this.initialPositions.Select(x => new Elf(x)).ToList();

		var currentPositions = this.initialPositions.ToHashSet();

		for (var i = 1; i <= 10; ++i)
		{
			var proposals = elves
				.Select(x => (Elf: x, Proposal: x.ConsiderMovement(currentPositions)))
				.ToList();

			var counts = new Counter<Point>(proposals.Select(x => x.Proposal));

			foreach (var (elf, proposal) in proposals)
			{
				if (counts[proposal] > 1) continue;
				elf.Move(proposal);
			}

			currentPositions = elves.Select(x => x.CurrentLocation).ToHashSet();
		}

		var xMin = currentPositions.Select(p => p.X).Min();
		var xMax = currentPositions.Select(p => p.X).Max();
		var width = xMax - xMin + 1;

		var yMin = currentPositions.Select(p => p.Y).Min();
		var yMax = currentPositions.Select(p => p.Y).Max();
		var height = yMax - yMin + 1;

		return (width * height) - currentPositions.Count;
	}

	public int Second()
	{
		var elves = this.initialPositions.Select(x => new Elf(x)).ToList();

		var currentPositions = this.initialPositions.ToHashSet();

		for (var i = 1; true; ++i)
		{
			var proposals = elves
				.Select(x => (Elf: x, Proposal: x.ConsiderMovement(currentPositions)))
				.ToList();

			if (proposals.All(x => x.Elf.CurrentLocation == x.Proposal))
				return i;

			var counts = new Counter<Point>(proposals.Select(x => x.Proposal));

			foreach (var (elf, proposal) in proposals)
			{
				if (counts[proposal] > 1) continue;
				elf.Move(proposal);
			}

			currentPositions = elves.Select(x => x.CurrentLocation).ToHashSet();
		}

		throw new InvalidOperationException("Should have completed");
	}

	private static void PrintMap(IReadOnlySet<Point> points)
	{
		var xMin = points.Select(p => p.X).Min();
		var xMax = points.Select(p => p.X).Max();

		var yMin = points.Select(p => p.Y).Min();
		var yMax = points.Select(p => p.Y).Max();

		var builder = new StringBuilder();

		for (var y = yMax; y >= yMin; --y)
		{
			for (var x = xMin; x <= xMax; ++x)
			{
				builder.Append(points.Contains((x, y)) ? '#' : '.');
			}
			builder.AppendLine();
		}


		Console.WriteLine(builder.ToString());
	}
	
	private class Elf
	{
		private Direction lastDirection = Direction.Right;
		
		public Elf(Point point)
		{
			this.CurrentLocation = point;
		}

		public Point CurrentLocation { get; private set; }

		public Point ConsiderMovement(HashSet<Point> currentMap)
		{
			try
			{
				var nextDirection = this.lastDirection;
				var proposedLocation = this.CurrentLocation;

				var up = this.CurrentLocation.Up;
				var upLeft = this.CurrentLocation.UpLeft;
				var upRight = this.CurrentLocation.UpRight;
				var down = this.CurrentLocation.Down;
				var downRight = this.CurrentLocation.DownRight;
				var downLeft = this.CurrentLocation.DownLeft;
				var left = this.CurrentLocation.Left;
				var right = this.CurrentLocation.Right;

				if (!currentMap.ContainsAny(up, upRight, right, downRight, down, downLeft, left, upLeft))
					return this.CurrentLocation;

				do
				{
					nextDirection = GetNextDirection(nextDirection);

					if (nextDirection == Direction.Up)
					{
						if (currentMap.ContainsAny(upLeft, up, upRight))
							continue;

						return up;
					}

					if (nextDirection == Direction.Down)
					{
						if (currentMap.ContainsAny(downRight, down, downLeft))
							continue;

						return down;
					}

					if (nextDirection == Direction.Left)
					{
						if (currentMap.ContainsAny(downLeft, left, upLeft))
							continue;

						return left;
					}

					if (nextDirection == Direction.Right)
					{
						if (currentMap.ContainsAny(upRight, right, downRight))
							continue;

						return right;
					}
				}
				while (nextDirection != this.lastDirection);

				return this.CurrentLocation;
			}
			finally
			{
				this.lastDirection = GetNextDirection(this.lastDirection);
			}
		}

		public void Move(Point point)
		{
			this.CurrentLocation = point;
		}

		private static Direction GetNextDirection(Direction previous) => previous switch
		{
			Direction.Right => Direction.Up,
			Direction.Up => Direction.Down,
			Direction.Down => Direction.Left,
			Direction.Left => Direction.Right,
			_ => throw new ArgumentOutOfRangeException(nameof(previous))
		};
	}
}
using System.Text.RegularExpressions;
using Adventageous.DataModel;
using Adventageous.Enums;
using Adventageous.Extensions;

namespace Adventageous.Days;

public class Day09
{
	private readonly List<Instruction> instructions;

	public Day09(Stream input)
	{
		using var reader = new StreamReader(input);

		var list = new List<Instruction?>();
		
		while (!reader.EndOfStream)
		{
			list.Add(Instruction.From(reader.ReadLine()));
		}

		this.instructions = list.SelectNotNull().ToList();
	}

	public int First()
	{
		var visited = new HashSet<Point>();
		var head = Point.Origin;
		var tail = Point.Origin;

		visited.Add(tail);

		foreach (var instruction in
			from instruction in this.instructions
			from _ in Enumerable.Range(1, instruction.Steps)
			select instruction)
		{
			head = head.Move(instruction.Direction);
			tail = tail.Chase(head);
			visited.Add(tail);
		}

		return visited.Count;
	}

	public int Second()
	{
		const int size = 10;
		
		var visited = new HashSet<Point>();
		var knots = Enumerable.Repeat(Point.Origin, size).ToArray();

		visited.Add(knots[^1]);

		foreach (var instruction in
			from instruction in this.instructions
			from _ in Enumerable.Range(1, instruction.Steps)
			select instruction)
		{
			knots[0] = knots[0].Move(instruction.Direction);

			for (var i = 1; i < 10; ++i)
			{
				knots[i] = knots[i].Chase(knots[i - 1]);
			}
			
			visited.Add(knots[^1]);
		}

		return visited.Count;
	}

	private struct Instruction
	{
		private static readonly Regex Matcher = new(@"(?<direction>[UDRL]) (?<steps>\d+)");
		
		public Direction Direction;
		public int Steps;

		public Instruction(Direction direction, int steps)
		{
			this.Direction = direction;
			this.Steps = steps;
		}

		public static Instruction? From(string input)
		{
			var match = Matcher.Match(input);

			if (!match.Success)
				return null;

			if (!int.TryParse(match.Groups["steps"].Value, out var steps))
				return null;

			Direction? direction = match.Groups["direction"].Value switch
			{
				"U" => Direction.Up,
				"D" => Direction.Down,
				"L" => Direction.Left,
				"R" => Direction.Right,
				_ => null,
			};

			if (!direction.HasValue)
				return null;

			return new Instruction(direction.Value, steps);
		}
	}
}

file static class PointExtensions09
{
	public static Point Chase(this Point self, Point that)
	{
		var difference = that - self;

		return (difference.X, difference.Y) switch
		{
			(>= -1 and <= 1, >= -1 and <= 1) =>
				self,
			_ =>
				new Point(self.X + Math.Sign(difference.X), self.Y + Math.Sign(difference.Y))
		};
	}

	public static Point Move(this Point self, Direction direction)
	{
		return direction switch
		{
			Direction.Up => new Point(self.X, self.Y + 1),
			Direction.Down => new Point(self.X, self.Y - 1),
			Direction.Left => new Point(self.X - 1, self.Y),
			Direction.Right => new Point(self.X + 1, self.Y),
			_ => throw new InvalidOperationException($"Direction {Enum.GetName(direction)} not supported"),
		};
	}
}
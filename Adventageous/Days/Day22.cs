using Adventageous.DataStructure;
using Adventageous.Enums;
using Adventageous.Extensions;

namespace Adventageous.Days;

public class Day22
{
	private readonly IReadOnlyDictionary<Point, Tile> map;
	private readonly IReadOnlyDictionary<int, MinMax<int>> rowMinMax;
	private readonly IReadOnlyDictionary<int, MinMax<int>> columnMinMax;

	private readonly IReadOnlyList<IInstruction> instructions;

	private readonly Cursor cursor;

	public Day22(Stream input)
	{
		var reader = new StreamReader(input);
		var map = new Dictionary<Point, Tile>();
		var rowMinMax = new Dictionary<int, MinMax<int>>();
		var columnMinMax = new Dictionary<int, MinMax<int>>();

		foreach (var (line, y) in reader.Lines().Indexed(1))
		{
			if (string.IsNullOrWhiteSpace(line)) break;

			foreach (var (character, x) in line.Indexed(1))
			{
				Tile? tile = character switch
				{
					'.' => Tile.Open,
					'#' => Tile.Wall,
					_ => null
				};

				if (!tile.HasValue)
					continue;

				map.Add((x, y), tile.Value);
				rowMinMax.Consume(y, x);
				columnMinMax.Consume(x, y);
			}
		}

		var lastLine = reader.ReadLine();
		if (lastLine is null) throw new InvalidOperationException("Line of directions should still be available");

		var turnIndices = lastLine.AllIndicesOfAny(new[] { 'L', 'R' }).ToList();
		var last = 0;

		var instructions = new List<IInstruction>(turnIndices.Count + 1);
		int steps;

		foreach (var index in turnIndices)
		{
			var length = index - last;
			if (int.TryParse(lastLine.AsSpan(last, length), out steps))
				instructions.Add(new MoveForward(steps));
			instructions.Add(Rotate.From(lastLine[index]));
			last = index + 1;
		}

		if (int.TryParse(lastLine.AsSpan(last), out steps))
			instructions.Add(new MoveForward(steps));

		this.map = map;
		this.rowMinMax = rowMinMax;
		this.columnMinMax = columnMinMax;

		this.instructions = instructions;

		this.cursor = new Cursor((this.rowMinMax.GetMin(1), 1));
	}

	public int First()
	{
		foreach (var instruction in this.instructions)
		{
			this.TransformCursor(instruction);
		}

		var directionPoints = this.cursor.Heading switch
		{
			Direction.Up => 1,
			Direction.Down => 3,
			Direction.Left => 2,
			Direction.Right => 0,
			_ => throw new ArgumentOutOfRangeException()
		};

		var (x, y) = this.cursor.Position;

		return 1_000 * y + 4 * x + directionPoints;
	}

	public int Second(int cubeSize)
	{
		return int.MinValue;
	}

	private void TransformCursor(IInstruction instruction)
	{
		instruction.Transform(
			this.cursor,
			this.map,
			this.rowMinMax,
			this.columnMinMax);
	}

	private enum Tile
	{
		Open,
		Wall,
	}

	private class Cursor
	{
		public Cursor(Point position)
		{
			this.Position = position;
			this.Heading = Direction.Right;
		}

		public Point Position { get; set; }

		public Direction Heading { get; set; }

		public Point GetFacingPoint(
			IReadOnlyDictionary<Point, Tile> map,
			IReadOnlyDictionary<int, MinMax<int>> rowMinMax,
			IReadOnlyDictionary<int, MinMax<int>> columnMinMax)
		{
			var candidate = this.Position[this.Heading];
			if (map.ContainsKey(candidate))
				return candidate;

			return this.Heading switch
			{
				// Math in this plane is left handed (y increases downward), we must flip up and down
				Direction.Up => (this.Position.X, columnMinMax.GetMin(this.Position.X)),
				Direction.Down => (this.Position.X, columnMinMax.GetMax(this.Position.X)),
				Direction.Left => (rowMinMax.GetMax(this.Position.Y), this.Position.Y),
				Direction.Right => (rowMinMax.GetMin(this.Position.Y), this.Position.Y),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public Point GetFacingPointCube(
			IReadOnlyDictionary<Point, Tile> map,
			IReadOnlyDictionary<int, MinMax<int>> rowMinMax,
			IReadOnlyDictionary<int, MinMax<int>> columnMinMax)
		{
			var candidate = this.Position[this.Heading];
			if (map.ContainsKey(candidate))
				return candidate;
		}
	}

	private interface IInstruction
	{
		void Transform(
			Cursor cursor,
			IReadOnlyDictionary<Point, Tile> map,
			IReadOnlyDictionary<int, MinMax<int>> rowMinMax,
			IReadOnlyDictionary<int, MinMax<int>> columnMinMax);

		void TransformCube(
			Cursor cursor,
			IReadOnlyDictionary<Point, Tile> map,
			IReadOnlyDictionary<int, MinMax<int>> rowMinMax,
			IReadOnlyDictionary<int, MinMax<int>> columnMinMax);
	}

	private readonly struct MoveForward : IInstruction
	{
		private readonly int amount;

		public MoveForward(int amount)
		{
			this.amount = amount;
		}

		public void Transform(
			Cursor cursor,
			IReadOnlyDictionary<Point, Tile> map,
			IReadOnlyDictionary<int, MinMax<int>> rowMinMax,
			IReadOnlyDictionary<int, MinMax<int>> columnMinMax)
		{
			foreach (var _ in Interval.HalfOpen(0, this.amount))
			{
				var facingPoint = cursor.GetFacingPoint(map, rowMinMax, columnMinMax);
				var facingTile = map[facingPoint];

				if (facingTile == Tile.Wall)
					break;
				cursor.Position = facingPoint;
			}
		}

		public void TransformCube(
			Cursor cursor,
			IReadOnlyDictionary<Point, Tile> map,
			IReadOnlyDictionary<int, MinMax<int>> rowMinMax,
			IReadOnlyDictionary<int, MinMax<int>> columnMinMax)
		{
			foreach (var _ in Interval.HalfOpen(0, this.amount))
			{
				var facingPoint = cursor.GetFacingPoint(map, rowMinMax, columnMinMax);
				var facingTile = map[facingPoint];

				if (facingTile == Tile.Wall)
					break;
				cursor.Position = facingPoint;
			}
		}
	}

	private readonly struct Rotate : IInstruction
	{
		private readonly Turn turn;

		public Rotate(Turn turn)
		{
			this.turn = turn;
		}

		public void Transform(
			Cursor cursor,
			IReadOnlyDictionary<Point, Tile> map,
			IReadOnlyDictionary<int, MinMax<int>> rowMinMax,
			IReadOnlyDictionary<int, MinMax<int>> columnMinMax)
		{
			cursor.Heading = cursor.Heading.Turn(this.turn);
		}

		public void TransformCube(
			Cursor cursor,
			IReadOnlyDictionary<Point, Tile> map,
			IReadOnlyDictionary<int, MinMax<int>> rowMinMax,
			IReadOnlyDictionary<int, MinMax<int>> columnMinMax) =>
			this.Transform(cursor, map, rowMinMax, columnMinMax);

		public static Rotate From(char character) => character switch
		{
			// Math in this plane is left handed (y increases downward), we must flip the turn direction
			'L' => new Rotate(Turn.Clockwise),
			'R' => new Rotate(Turn.Widdershins),
			_ => throw new ArgumentOutOfRangeException($"Cannot create a turn from '{character}'")
		};
	}
}
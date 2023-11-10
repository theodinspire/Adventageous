using System.Diagnostics;
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

	public Day22(Stream input)
	{
		var reader = new StreamReader(input);
		var map = new Dictionary<Point, Tile>();
		var rowMinMax = new Dictionary<int, MinMax<int>>();
		var columnMinMax = new Dictionary<int, MinMax<int>>();

		foreach (var (line, x) in reader.Lines().Indexed())
		{
			if (string.IsNullOrWhiteSpace(line)) break;

			foreach (var (character, y) in line.Indexed())
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
			instructions.Add(Rotation.From(lastLine[index]));
			last = index + 1;
		}

		if (int.TryParse(lastLine.AsSpan(last), out steps))
			instructions.Add(new MoveForward(steps));

		this.map = map;
		this.rowMinMax = rowMinMax;
		this.columnMinMax = columnMinMax;

		this.instructions = instructions;
	}

	public int First()
	{
		var cursor = new Cursor((0, this.columnMinMax.GetMin(0)));

		foreach (var instruction in this.instructions)
		{
			this.Transform(cursor, instruction);
		}

		return Score(cursor);
	}

	public int Second(int cubeSize)
	{
		var cube = new Cube(this.map, cubeSize);
		var cursor = new Cursor((0, this.columnMinMax.GetMin(0)));

		foreach (var instruction in this.instructions)
		{
			if (instruction is Rotation rotation)
			{
				this.Rotate(cursor, rotation.Turn);
				continue;
			}

			if (instruction is not MoveForward translation)
				throw new InvalidOperationException("Not a valid instruction");

			foreach (var _ in Interval.HalfOpen(0, translation.Amount))
			{
				var facingPoint = this.GetFacingPoint(cursor);
				if (facingPoint.HasValue)
				{
					var facingTile = this.map[facingPoint.Value];
					if (facingTile == Tile.Wall)
						break;
					cursor.Position = facingPoint.Value;
					continue;
				}

				var (nextCubePoint, newDirection) = cube.GetNextPointAndDirection(cursor);

				var nextTile = this.map[nextCubePoint];
				if (nextTile == Tile.Wall)
					break;
				cursor.Position = nextCubePoint;
				cursor.Heading = newDirection;
			}
		}

		return Score(cursor);
	}

	private void Transform(Cursor cursor, IInstruction instruction)
	{
		switch (instruction)
		{
			case Rotation r:
				this.Rotate(cursor, r.Turn);
				break;
			case MoveForward m:
				this.Translate(cursor, m);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(instruction), "Cannot determine how to transform cursor");
		}
	}

	private void Rotate(Cursor cursor, Turn turn)
	{
		cursor.Heading = cursor.Heading.Turn(turn);
	}

	private void Translate(Cursor cursor, MoveForward instruction)
	{
		foreach (var _ in Interval.HalfOpen(0, instruction.Amount))
		{
			var facingPoint = this.GetFacingPointFlat(cursor);
			var facingTile = this.map[facingPoint];

			if (facingTile == Tile.Wall)
				break;
			cursor.Position = facingPoint;
		}
	}

	private Point? GetFacingPoint(Cursor cursor)
	{
		var candidate = cursor.Position[cursor.Heading];
		if (this.map.ContainsKey(candidate))
			return candidate;
		return null;
	}

	private Point GetFacingPointFlat(Cursor cursor)
	{
		var p = this.GetFacingPoint(cursor);
		if (p is not null)
			return p.Value;

		var (x, y) = cursor.Position;

		return cursor.Heading switch
		{
			Direction.Down => (x, this.columnMinMax.GetMax(x)),
			Direction.Up => (x, this.columnMinMax.GetMin(x)),
			Direction.Left => (this.rowMinMax.GetMax(y), y),
			Direction.Right => (this.rowMinMax.GetMin(y), y),
			_ => throw new ArgumentOutOfRangeException(nameof(cursor))
		};
	}

	private static int Score(Cursor cursor)
	{
		var directionPoints = cursor.Heading switch
		{
			Direction.Up => 0,
			Direction.Right => 1,
			Direction.Down => 2,
			Direction.Left => 3,
			_ => throw new ArgumentOutOfRangeException(nameof(cursor))
		};

		var (y, x) = cursor.Position;

		return 1_000 * (y + 1) + 4 * (x + 1) + directionPoints;
	}

	private enum Tile
	{
		Open,
		Wall,
	}

	[DebuggerDisplay("{Position} {Heading}")]
	private class Cursor
	{
		public Cursor(Point position)
		{
			this.Position = position;
			this.Heading = Direction.Up;
		}

		public Point Position { get; set; }

		public Direction Heading { get; set; }
	}

	private interface IInstruction
	{
	}

	[DebuggerDisplay("Move {Amount}")]
	private readonly struct MoveForward : IInstruction
	{
		public readonly int Amount;

		public MoveForward(int amount)
		{
			this.Amount = amount;
		}
	}

	[DebuggerDisplay("Turn {Turn}")]
	private readonly struct Rotation : IInstruction
	{
		public readonly Turn Turn;

		public Rotation(Turn turn)
		{
			this.Turn = turn;
		}

		public static Rotation From(char character) => character switch
		{
			'L' => new Rotation(Turn.Widdershins),
			'R' => new Rotation(Turn.Clockwise),
			_ => throw new ArgumentOutOfRangeException($"Cannot create a turn from '{character}'")
		};
	}

	[DebuggerDisplay("{lowerLeft}")]
	private class Face
	{
		private readonly int length;

		private readonly Point lowerLeft;

		private readonly Dictionary<Direction, (Face neighbor, Direction newDirection)> neighbors =
			new Dictionary<Direction, (Face neighbor, Direction newDirection)>(4);

		public Face(int length, Point lowerLeft)
		{
			this.length = length;
			this.lowerLeft = lowerLeft;
		}

		public void AddNeighbor(Direction direction, Face neighbor, Direction newDirection)
		{
			if (!this.neighbors.TryGetValue(direction, out var existing))
			{
				this.neighbors.Add(direction, (neighbor, newDirection));
				return;
			}

			if (!ReferenceEquals(neighbor, existing.neighbor))
				throw new ArgumentException("Another neighbor already exists in this direction.", nameof(neighbor));

			if (newDirection != existing.newDirection)
				throw new ArgumentException("This neighbor is already matched at this point, but the transformed location does not match", nameof(newDirection));
		}

		public IReadOnlyDictionary<Direction, (Face neighbor, Direction newDirection)> Neighbors => this.neighbors;

		public (Point, Direction) GetNextPointAndDirection(Cursor cursor)
		{
			var (x, y) = ModuloTransform(cursor.Position, this.length);

			var (neighbor, newDirection) = this.neighbors[cursor.Heading];

			Point rotated;

			switch (cursor.Heading, newDirection)
			{
				case (Direction.Up, Direction.Up):
				case (Direction.Right, Direction.Right):
				case (Direction.Down, Direction.Down):
				case (Direction.Left, Direction.Left):
					rotated = (x, y);
					break;

				case (Direction.Up, Direction.Right):
				case (Direction.Right, Direction.Down):
				case (Direction.Down, Direction.Left):
				case (Direction.Left, Direction.Up):
					rotated = (y, TurnDimension(x));
					break;

				case (Direction.Up, Direction.Down):
				case (Direction.Right, Direction.Left):
				case (Direction.Down, Direction.Up):
				case (Direction.Left, Direction.Right):
					rotated = (TurnDimension(x), TurnDimension(y));
					break;

				case (Direction.Up, Direction.Left):
				case (Direction.Right, Direction.Up):
				case (Direction.Down, Direction.Right):
				case (Direction.Left, Direction.Down):
					rotated = (TurnDimension(y), x);
					break;

				default:
					throw new IndexOutOfRangeException("Could not rotate relative point");
			}

			var phantomAnchor = neighbor.lowerLeft.Over(newDirection.Opposite(), this.length);

			return (rotated[newDirection] + phantomAnchor, newDirection);

			int TurnDimension(int n) => (this.length - 1) - n;
		}
	}

	private class Cube
	{
		private readonly int length;

		private IReadOnlyDictionary<Point, Face> Faces;

		public Cube(IReadOnlyDictionary<Point, Tile> map, int length)
		{
			this.length = length;
			var faces = new Dictionary<Point, Face>(6);

			for (var i = 0; i < 4; ++i)
			{
				for (var j = 0; j < 4; ++j)
				{
					var x = i * this.length;
					var y = j * this.length;
					var p = new Point(x, y);

					if (!map.ContainsKey(p))
						continue;

					var face = new Face(this.length, p);
					faces.Add(p, face);

					var d = p.Over(Direction.Down, this.length);
					if (faces.TryGetValue(d, out var down) && down != null)
					{
						face.AddNeighbor(Direction.Down, down, Direction.Down);
						down.AddNeighbor(Direction.Up, face, Direction.Up);
					}

					var l = p.Over(Direction.Left, this.length);
					if (faces.TryGetValue(l, out var left) && left != null)
					{
						face.AddNeighbor(Direction.Left, left, Direction.Left);
						left.AddNeighbor(Direction.Right, face, Direction.Right);
					}
				}
			}

			foreach (var (_, face) in faces)
			{
				foreach (var direction in face.Neighbors.Keys.ToArray())
				{
					var (neighbor, neighborsRelativeDirection) = face.Neighbors[direction];
					var clockwise = direction.TurnClockwise();
					var widdershins = direction.TurnWiddershins();

					// Clockwise
					var neighborsClockwise = neighborsRelativeDirection.TurnClockwise();
					var opposite = direction.Opposite();

					if (!face.Neighbors.ContainsKey(clockwise) &&
					    neighbor.Neighbors.TryGetValue(neighborsClockwise, out var thirdClockwise))
					{
						var thirdClockwiseClockwise = thirdClockwise.newDirection.TurnClockwise();
						thirdClockwise.neighbor.AddNeighbor(thirdClockwiseClockwise, face, widdershins);
						var thirdClockwiseWiddershins = thirdClockwise.newDirection.TurnWiddershins();
						face.AddNeighbor(clockwise, thirdClockwise.neighbor, thirdClockwiseWiddershins);
					}

					// Widdershins
					var neighborsWiddershins = neighborsRelativeDirection.TurnWiddershins();

					if (!face.Neighbors.ContainsKey(widdershins) &&
					    neighbor.Neighbors.TryGetValue(neighborsWiddershins, out var thirdWiddershins))
					{
						var thirdWiddershinsWiddershins = thirdWiddershins.newDirection.TurnWiddershins();
						thirdWiddershins.neighbor.AddNeighbor(thirdWiddershinsWiddershins, face, clockwise);
						var thirdWiddershinsClockwise = thirdWiddershins.newDirection.TurnClockwise();
						face.AddNeighbor(widdershins, thirdWiddershins.neighbor, thirdWiddershinsClockwise);
					}
				}
			}

			this.Faces = faces;
		}

		public (Point, Direction) GetNextPointAndDirection(Cursor cursor)
		{
			var upperLeft = cursor.Position - ModuloTransform(cursor.Position, this.length);

			if (!this.Faces.TryGetValue(upperLeft, out var face))
				throw new ArgumentOutOfRangeException(nameof(cursor), "Cursor not found on cube");

			return face.GetNextPointAndDirection(cursor);
		}
	}

	private static Point ModuloTransform(Point point, int divisor) =>
		(point.X % divisor, point.Y % divisor);

	private static Direction[] CardinalDirections = new[]
	{
		Direction.Up, Direction.Right, Direction.Left, Direction.Down
	};
}
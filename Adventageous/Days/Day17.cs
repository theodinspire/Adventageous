using System.Text;
using Adventageous.DataStructure;
using Adventageous.Extensions;

namespace Adventageous.Days;

public class Day17
{
	private const long RockCount = 1_000_000_000_000;

	private readonly IReadOnlyList<Jet> Jets;

	private readonly RockGenerator generator = new RockGenerator();
	private readonly Shaft shaft;

	private readonly Dictionary<(int jetIndex, byte rockIndex), (long Step, int Rock, int Height)> strikes =
		new Dictionary<(int jetIndex, byte rockIndex), (long Step, int Rock, int Height)>();

	private HashSet<long> previouslyLoopedRocks = new HashSet<long>();

	private (int jetIndex, byte rockIndex)? loopKey;
	private (long Step, long Rock, int Height)? loopTerminus;

	private long step = 0;

	public Day17(Stream input)
	{
		using var reader = new StreamReader(input);
		var line = reader.ReadLine();
		if (line is null) throw new InvalidDataException("Input stream empty");

		this.Jets = line.SelectNotNull(Map).ToList();
		this.shaft = new Shaft(this.generator);
	}

	public int First()
	{
		while (this.generator.RocksGenerated <= 2022)
		{
			this.Iterate();
		}

		return this.shaft.Height;
	}

	public long Second()
	{
		while (this.loopKey is null)
		{
			this.Iterate();
		}

		if (this.loopKey is null)
			return long.MinValue;
		if (this.loopTerminus is not var (terminusStep, terminusRock, terminusHeight))
			return long.MinValue;

		Console.WriteLine($"Loop found at step {terminusStep:N0}, rock {terminusRock:N0}, height {terminusHeight:N0}");
		var loopStart = this.strikes[this.loopKey.Value];
		Console.WriteLine($"Loop starts at step {loopStart.Step:N0}, rock {loopStart.Rock:N0}, height {loopStart.Height:N0}");

		var finalRockMinusOffset = RockCount - loopStart.Rock;
		var loopInRocks = terminusRock - loopStart.Rock;
		var loopHeight = terminusHeight - loopStart.Height;

		var (loops, remainder) = Math.DivRem(finalRockMinusOffset, loopInRocks);

		var targetRock = loopStart.Rock + remainder;
		var targetRockHeight = this.strikes.Values.FirstOrDefault(x => x.Rock == targetRock).Height;

		return targetRockHeight + (loops * loopHeight) - 1; //Off by one?
	}

	private void Iterate()
	{
		var current = this.step++;
		var rock = this.generator.RocksGenerated - 1;
		var index = (int)(current % this.Jets.Count);
		this.shaft.Step(this.Jets[index]);

		if (!this.shaft.JustSettled)
			return;

		var key = (index, this.generator.PreviousLoopIndex);
		var value = (Step: current, Rock: rock, Height: this.shaft.Height);

		var containsKey = this.strikes.ContainsKey(key);
		var n = RockGenerator.Count * 2;
		var lastNRocks = Interval.HalfOpen(rock - n, rock);

		if (containsKey && lastNRocks.All(x => this.previouslyLoopedRocks.Contains(x)))
		{
			this.loopKey = key;
			this.loopTerminus = value;
		}
		else if (containsKey)
		{
			this.previouslyLoopedRocks.Add(rock);
			this.strikes[key] = value;
		}
		else
			this.strikes.Add(key, value);
	}

	private static Jet? Map(char c) => c switch
	{
		'<' => Jet.Left,
		'>' => Jet.Right,
		_ => null
	};

	private enum Jet
	{
		Left,
		Right
	}

	private class Shaft
	{
		// Width of shaft
		private const int Width = 7;

		// Amount of space above the tower
		private const int Overhead = 4;

		private readonly HashSet<Point> tower = new HashSet<Point>();
		private readonly RockGenerator generator;
		private Rock current;

		public Shaft(RockGenerator generator)
		{
			this.generator = generator;
			this.current = generator.Next(this.InsertPoint);
		}

		public int Height { get; private set; } = 0;

		public bool JustSettled { get; private set; } = false;

		private Point InsertPoint => (2, this.Height + Overhead);

		public void Step(Jet jet)
		{
			if (this.current.GetPotentialPoints(jet).All(this.IsOpen))
				this.current.Push(jet);

			if (this.current.PointsAdjacentToBottom.All(this.IsOpen))
			{
				this.current.Descend();
				this.JustSettled = false;
				return;
			}

			foreach (var point in this.current.Body)
			{
				this.tower.Add(point);
				this.Height = Math.Max(point.Y, this.Height);
			}

			this.current = this.generator.Next(this.InsertPoint);
			this.JustSettled = true;
		}

		private bool IsOpen(Point point)
		{
			if (point.X < 0)
				return false;
			if (point.Y < 1)
				return false;
			if (point.X >= Width)
				return false;

			return !this.tower.Contains(point);
		}

		public void Print()
		{
			Console.WriteLine(this.Layout());
			Console.WriteLine();
		}

		public string Layout()
		{
			var rockBody = this.current.Body.ToHashSet();

			var maxHeight = rockBody.Max(x => x.Y);
			const int lineWidth = Width + 4;

			var builder = new StringBuilder(maxHeight * lineWidth);

			char GetCharacter((int x, int y) p)
			{
				if (this.tower.Contains(p)) return '#';
				if (rockBody.Contains(p)) return '@';
				return '.';
			}

			for (var y = maxHeight; y > 0; --y)
			{
				builder.Append('|');

				foreach (var c in Interval.HalfOpen(0, Width)
					.Select(x => (x, y))
					.Select(GetCharacter))
				{
					builder.Append(c);
				}

				builder.AppendLine("|");
			}

			builder.AppendLine("+-------+");

			return builder.ToString();
		}
	}

	private class Rock
	{
		public readonly int Height;
		public readonly int Width;

		private readonly IReadOnlySet<Point> shape;

		private readonly IReadOnlyCollection<Point> relativeRight;
		private readonly IReadOnlyCollection<Point> relativeLeft;
		private readonly IReadOnlyCollection<Point> relativeBottom;

		/// <summary>
		/// Creates a rock based on a collection of relative points at some position.
		/// </summary>
		/// <param name="shape">A collection of points, such that (0,0) would be the bottom left of the rock.</param>
		/// <param name="position">The position of the rock in its containing <see cref="Shaft"/>.</param>
		/// <remarks></remarks>
		public Rock(IReadOnlySet<Point> shape, Point position)
		{
			this.shape = shape;
			this.Position = position;

			this.Height = this.shape.Select(p => p.Y).Max() + 1;

			var right = this.shape.GroupBy(p => p.Y)
				.Select(g => (Point)(g.Select(p => p.X).Max() + 1, g.Key))
				.ToHashSet();
			this.Width = right.Select(p => p.X).Max();
			this.relativeRight = right;

			this.relativeLeft = this.shape.GroupBy(p => p.Y)
				.Select(g => (Point)(g.Select(p => p.X).Min() - 1, g.Key))
				.ToHashSet();

			this.relativeBottom = this.shape.GroupBy(p => p.X)
				.Select(g => (Point)(g.Key, g.Select(p => p.Y).Min() - 1))
				.ToHashSet();
		}

		/// <summary>
		/// A value indicating the bottom left side of the rock
		/// </summary>
		public Point Position { get; private set; }

		public IEnumerable<Point> Body => this.shape.Select(p => p + this.Position);

		public IEnumerable<Point> PointsAdjacentToRight => this.relativeRight.Select(p => p + this.Position);

		public IEnumerable<Point> PointsAdjacentToLeft => this.relativeLeft.Select(p => p + this.Position);

		public IEnumerable<Point> PointsAdjacentToBottom => this.relativeBottom.Select(p => p + this.Position);

		public void Descend() => this.Position = this.Position.Down;

		public void Push(Jet direction) => this.Position = direction switch
			{
				Jet.Left => this.Position.Left,
				Jet.Right => this.Position.Right,
				_ => this.Position
			};

		public IEnumerable<Point> GetPotentialPoints(Jet jet) => jet switch
			{
				Jet.Left => this.PointsAdjacentToLeft,
				Jet.Right => this.PointsAdjacentToRight,
				_ => throw new InvalidOperationException("No direction to probe.")
			};

		/// <summary>
		/// Parses a multiline string into the collection of points defining a rock.
		/// </summary>
		/// <param name="shape">The string representing the shape of the rock, of the format:
		/// <code>
		/// @".#.
		/// ###
		/// .#."
		/// </code></param>
		/// <returns>The set of points relative to the rock's bottom left, (0, 0), that a rock occupies</returns>
		/// <remarks>The string will be split along lines, and white space trimmed from each line. Any '#' will be converted into a point;
		/// all other characters will be ignored.</remarks>
		public static IReadOnlySet<Point> Parse(string shape)
		{
			var set = new HashSet<Point>(shape.Length);

			var lines = shape.Split(Environment.NewLine)
				.Select(x => x.Trim())
				.Where(x => x != string.Empty)
				.ToArray();

			var top = lines.Length - 1;
			for (var i = 0; i < lines.Length; ++i)
			{
				var line = lines[i];
				for (var j = 0; j < line.Length; ++j)
				{
					if (line[j] != '#') continue;
					set.Add((j, top - i));
				}
			}

			set.TrimExcess();
			return set;
		}
	}

	private class RockGenerator
	{
		private static IReadOnlySet<Point>[] Shapes = new[]
		{
			"####",

			"""
			.#.
			###
			.#.
			""",

			"""
			..#
			..#
			###
			""",

			"""
			#
			#
			#
			#
			""",

			"""
			##
			##
			"""
		}.Select(Rock.Parse).ToArray();

		public static byte Count => (byte)Shapes.Length;

		public RockGenerator()
		{
		}

		public int RocksGenerated { get; private set; } = 0;

		public byte PreviousLoopIndex => (byte)((this.RocksGenerated - 1) % Count);

		public Rock Next(Point position)
		{
			var index = this.RocksGenerated++ % Shapes.Length;
			return new Rock(Shapes[index], position);
		}
	}
}
using Adventageous.Enums;

namespace Adventageous.DataStructure;

public readonly struct Point : IEquatable<Point>
{
	public Point(int x, int y)
	{
		this.X = x;
		this.Y = y;
	}
	
	public readonly int X;
	
	public readonly int Y;

	public override int GetHashCode()
	{
		return (this.X, this.Y).GetHashCode();
	}

	public override string ToString()
	{
		return $"({this.X}, {this.Y})";
	}

	#region Convenience Initializers

	/// <summary>
	/// Creates a point from a string pair of form "0,0"
	/// </summary>
	/// <param name="s">The string representation of the point.</param>
	/// <returns>The represented point.</returns>
	public static Point Parse(string s) => Parse(s, ",");

	/// <summary>
	/// Creates a point from a string pair, <em>e.g.</em> "0,0"
	/// </summary>
	/// <param name="s">The string representation of the point.</param>
	/// <param name="separator">The separating character.</param>
	/// <returns>The represented point.</returns>
	public static Point Parse(string s, string separator)
	{
		var bits = s.Split(separator);
		if (bits.Length < 2)
			throw new InvalidOperationException($"'{s}' is does not contain separator '{separator}'. Point cannot be created");

		if (!int.TryParse(bits[0], out var x))
			throw new InvalidOperationException($"'{bits[0]}' could not be parsed as a number");

		if (!int.TryParse(bits[1], out var y))
			throw new InvalidOperationException($"'{bits[1]}' could not be parsed as a number");

		return (x, y);
	}

	#endregion

	#region Conversion

	public static implicit operator (int x, int y)(Point p) => (p.X, p.Y);

	public static implicit operator Point((int x, int y) tuple) => new Point(tuple.x, tuple.y);

	#endregion

	#region Constant Points
	
	public static readonly Point Origin = (0, 0);

	#endregion

	#region Relative Points

	public Point Up    => new Point(this.X, this.Y + 1);
	public Point Down  => new Point(this.X, this.Y - 1);
	public Point Left  => new Point(this.X - 1, this.Y);
	public Point Right => new Point(this.X + 1, this.Y);

	public Point UpRight   => (this.X + 1, this.Y + 1);
	public Point DownRight => (this.X + 1, this.Y - 1);
	public Point DownLeft  => (this.X - 1, this.Y - 1);
	public Point UpLeft    => (this.X - 1, this.Y + 1);

	public Point[] TaxiCabNeighbors => new[] { this.Up, this.Right, this.Down, this.Left };

	public Point this[Direction direction] => direction switch
	{
		Direction.Up    => this.Up,
		Direction.Down  => this.Down,
		Direction.Left  => this.Left,
		Direction.Right => this.Right,
		_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
	};

	#endregion

	#region Operators

	public static Point operator +(Point self) => self;
	public static Point operator -(Point self) => (-self.X, -self.Y);

	public static Point operator +(Point left, Point right) =>
		(left.X + right.X, left.Y + right.Y);

	public static Point operator -(Point left, Point right) =>
		(left.X - right.X, left.Y - right.Y);

	#endregion

	#region Transformations

	public Point Transformed(Func<int, int, (int x, int y)> transformation)
	{
		return transformation(this.X, this.Y);
	}

	#endregion

	#region Equality

	public bool Equals(Point other)
	{
		return X == other.X && Y == other.Y;
	}

	public override bool Equals(object? obj)
	{
		return obj is Point other && Equals(other);
	}

	public static bool operator ==(Point left, Point right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Point left, Point right)
	{
		return !left.Equals(right);
	}

	#endregion
}
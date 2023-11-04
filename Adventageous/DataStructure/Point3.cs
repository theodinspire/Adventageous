namespace Adventageous.DataStructure;

public readonly struct Point3 : IEquatable<Point3>
{
	public Point3(int x, int y, int z)
	{
		this.X = x;
		this.Y = y;
		this.Z = z;
	}

	public readonly int X;

	public readonly int Y;

	public readonly int Z;

	public override int GetHashCode()
	{
		return (this.X, this.Y, this.Z).GetHashCode();
	}

	public override string ToString()
	{
		return $"({this.X}, {this.Y}, {this.Z})";
	}

	#region Convenience Initializers

	/// <summary>
	/// Creates a point from a string pair of form "0,0,0"
	/// </summary>
	/// <param name="s">The string representation of the point.</param>
	/// <returns>The represented point.</returns>
	public static Point3 Parse(string s) => Parse(s, ",");

	/// <summary>
	/// Creates a point from a string pair, <em>e.g.</em> "0,0,0"
	/// </summary>
	/// <param name="s">The string representation of the point.</param>
	/// <param name="separator">The separating character.</param>
	/// <returns>The represented point.</returns>
	public static Point3 Parse(string s, string separator)
	{
		var bits = s.Split(separator);
		if (bits.Length < 3)
			throw new InvalidOperationException($"'{s}' is does not contain separator '{separator}'. Point cannot be created");

		if (!int.TryParse(bits[0], out var x))
			throw new InvalidOperationException($"'{bits[0]}' could not be parsed as a number");

		if (!int.TryParse(bits[1], out var y))
			throw new InvalidOperationException($"'{bits[1]}' could not be parsed as a number");

		if (!int.TryParse(bits[2], out var z))
			throw new InvalidOperationException($"'{bits[2]}' could not be parsed as a number");

		return (x, y, z);
	}

	#endregion

	#region Conversion

	public static implicit operator (int x, int y, int z)(Point3 p) => (p.X, p.Y, p.Z);

	public static implicit operator Point3((int x, int y, int z) tuple) => new Point3(tuple.x, tuple.y, tuple.z);

	#endregion

	#region Constant Points

	public static readonly Point3 Origin = (0, 0, 0);

	#endregion

	#region Relative Points

	public Point3 Left     => (this.X - 1, this.Y, this.Z);

	public Point3 Right    => (this.X + 1, this.Y, this.Z);

	public Point3 Backward => (this.X, this.Y - 1, this.Z);

	public Point3 Forward  => (this.X, this.Y + 1, this.Z);

	public Point3 Down     => (this.X, this.Y, this.Z - 1);

	public Point3 Up       => (this.X, this.Y, this.Z + 1);

	public Point3[] TaxiCabNeighbors =>
		new[] { this.Left, this.Right, this.Backward, this.Forward, this.Down, this.Up };

	#endregion

	#region Operators

	public static Point3 operator +(Point3 self) => self;

	public static Point3 operator -(Point3 self) => (-self.X, -self.Y, -self.Z);

	public static Point3 operator +(Point3 left, Point3 right) =>
		(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

	public static Point3 operator -(Point3 left, Point3 right) =>
		(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

	public static Point3 operator *(int scalar, Point3 self) =>
		(scalar * self.X, scalar * self.Y, scalar * self.Z);

	#endregion

	#region Equality

	public bool Equals(Point3 other)
	{
		return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
	}

	public override bool Equals(object? obj)
	{
		return obj is Point3 other && this.Equals(other);
	}

	public static bool operator ==(Point3 left, Point3 right)
		=> left.Equals(right);

	public static bool operator !=(Point3 left, Point3 right)
		=> !left.Equals(right);

	#endregion

	#region Comparison

	/// <summary>
	/// Determines the <a href="https://en.wikipedia.org/wiki/Taxicab_geometry">Manhattan Distance</a> between this and another point.
	/// </summary>
	/// <param name="that">The other point.</param>
	/// <returns>Their Manhattan Distance.</returns>
	public int ManhattanDistance(Point3 that) =>
		ManhattanDistance(this, that);

	/// <summary>
	/// Determines the <a href="https://en.wikipedia.org/wiki/Taxicab_geometry">Manhattan Distance</a> between two points.
	/// </summary>
	/// <param name="left">One point</param>
	/// <param name="right">The other point</param>
	/// <returns>Their Manhattan Distance.</returns>
	public static int ManhattanDistance(Point3 left, Point3 right)
	{
		var h = Math.Abs(left.X - right.X);
		var d = Math.Abs(left.Y - right.Y);
		var v = Math.Abs(left.Z - right.Z);

		return h + d + v;
	}

	public bool IsAdjacent(Point3 that) => this.TaxiCabNeighbors.Contains(that);

	public static bool AreAdjacent(Point3 left, Point3 right) => left.IsAdjacent(right);

	#endregion
}
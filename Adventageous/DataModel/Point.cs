namespace Adventageous.DataModel;

public struct Point
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

	#region Operators

	public static Point operator +(Point self) => self;
	public static Point operator -(Point self) => new(-self.X, -self.Y);

	public static Point operator +(Point left, Point right) =>
		new(left.X + right.X, left.Y + right.Y);
	
	public static Point operator -(Point left, Point right) =>
		new(left.X - right.X, left.Y - right.Y);

	#endregion

	#region Constant Points
	
	public static readonly Point Origin = new(0, 0);

	#endregion

	#region Relative Points

	public Point Up    => new Point(this.X, this.Y + 1);
	public Point Down  => new Point(this.X, this.Y - 1);
	public Point Left  => new Point(this.X - 1, this.Y);
	public Point Right => new Point(this.X + 1, this.Y);

	public Point[] TaxiCabNeighbors => new[] { this.Up, this.Right, this.Down, this.Left };

	#endregion
}
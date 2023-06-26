namespace Adventageous.DataModel;

public struct Point
{
	public Point(int x, int y)
	{
		this.X = x;
		this.Y = y;
	}
	
	public int X;
	
	public int Y;

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
}
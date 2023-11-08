namespace Adventageous.Enums;

public static class DirectionExtensions
{
	public static Direction TurnClockwise(this Direction self) => self switch
	{
		Direction.Up => Direction.Right,
		Direction.Down => Direction.Left,
		Direction.Left => Direction.Up,
		Direction.Right => Direction.Down,
		Direction.UpRight => Direction.DownRight,
		Direction.DownRight => Direction.DownLeft,
		Direction.DownLeft => Direction.UpLeft,
		Direction.UpLeft => Direction.UpRight,
		_ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
	};

	public static Direction TurnWiddershins(this Direction self) => self switch
	{
		Direction.Up => Direction.Left,
		Direction.Down => Direction.Right,
		Direction.Left => Direction.Down,
		Direction.Right => Direction.Up,
		Direction.UpRight => Direction.UpLeft,
		Direction.DownRight => Direction.UpRight,
		Direction.DownLeft => Direction.DownRight,
		Direction.UpLeft => Direction.DownLeft,
		_ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
	};

	public static Direction Turn(this Direction self, Turn turn) => turn switch
	{
		Enums.Turn.Clockwise => self.TurnClockwise(),
		Enums.Turn.Widdershins => self.TurnWiddershins(),
		_ => throw new ArgumentOutOfRangeException(nameof(turn), turn, null)
	};
}
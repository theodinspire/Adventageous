using Adventageous.DataStructure;

namespace Adventageous.Tests.DataStructure;

using NUnit.Framework;

[TestFixture]
public class PointTests
{
	private static object[][] ManhattanDistanceTestValues = new[]
	{
		new object[] { Point.Origin, new Point(0, 0), 0 },
		new object[] { Point.Origin, new Point(5, 0), 5 },
		new object[] { Point.Origin, new Point(0, 5), 5 },
		new object[] { Point.Origin, new Point(5, 5), 10 },
	};

	[TestCaseSource(sourceName: nameof(ManhattanDistanceTestValues))]
	public void ManhattanDistance(Point left, Point right, int expected)
	{
		Assert.That(left.ManhattanDistance(right), Is.EqualTo(expected));
	}
}
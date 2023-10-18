namespace Adventageous.Tests.DataStructure;

using NUnit.Framework;
using Adventageous.DataStructure;

[TestFixture]
public class IntervalTests
{
	[Test]
	public void Count_ExpectedSizeForNegativeInterval_Closed()
	{
		var sut = Interval.Closed(-1, -10);

		Assert.That(sut, Has.Count.EqualTo(10));
	}

	[Test]
	public void Count_ExpectedSizeForNegativeInterval_HalfOpen()
	{
		var sut = Interval.HalfOpen(0, -10);

		Assert.That(sut, Has.Count.EqualTo(10));
	}

	[Test]
	public void Interval_Positive_IsOrdered()
	{
		var sut = Interval.HalfOpen(0, 10);

		Assert.That(sut, Is.Ordered.Ascending);
	}

	[Test]
	public void Interval_Negative_IsOrderedDescending()
	{
		var sut = Interval.HalfOpen(0, -10);

		Assert.That(sut, Is.Ordered.Descending);
	}

	[TestCase(1, 3, 2)]
	[TestCase(-1, -3, -2)]
	public void Interval_ContainsMidPoint(int start, int end, int mid)
	{
		var sut = Interval.Closed(start, end);

		Assert.That(sut, Has.Member(mid));
	}

	[Test]
	public void Interval_Equivalency_Closed_Positive()
	{
		var sut = Interval.Closed(1, 3);

		Assert.That(sut, Is.EquivalentTo(new[] {1, 2, 3}));
	}

	[Test]
	public void Interval_Equivalency_Closed_Negative()
	{
		var sut = Interval.Closed(-1, -3);

		Assert.That(sut, Is.EquivalentTo(new[] {-1, -2, -3}));
	}
}
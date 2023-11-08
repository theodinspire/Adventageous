using Adventageous.DataStructure;
using Adventageous.Extensions;

namespace Adventageous.Tests.Extensions;

using NUnit.Framework;

[TestFixture]
public class StringExtensionTests
{
	[Test]
	public void AllIndicesOfAny_ReturnsAllIndicesIfAllCharactersIncluded()
	{
		const string subject = "abc";

		var indices = subject.AllIndicesOfAny(subject.ToArray());

		Assert.That(indices, Is.EquivalentTo(Interval.HalfOpen(0, subject.Length)));
	}
}
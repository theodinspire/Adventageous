namespace Adventageous.Tests.Feature;

using NUnit.Framework;

[TestFixture]
public class StackFeatureTests
{
	[Test]
	public void StringJoin_LastInsertedFirstCharacter()
	{
		var stack = new Stack<char>(3);
		stack.Push('1');
		stack.Push('2');
		stack.Push('3');

		var actual = string.Join(string.Empty, stack);

		Assert.That(actual, Is.EqualTo("321"));
	}
}
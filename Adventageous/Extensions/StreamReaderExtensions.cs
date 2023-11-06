namespace Adventageous.Extensions;

public static class StreamReaderExtensions
{
	public static IEnumerable<string> Lines(this StreamReader self)
	{
		while (!self.EndOfStream)
		{
			var line = self.ReadLine();
			if (line is null) continue;
			yield return line;
		}
	}
}
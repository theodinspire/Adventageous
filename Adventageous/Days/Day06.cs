namespace Adventageous.Days;

public class Day06
{
	private List<string> Buffers = new();

	public Day06(Stream input)
	{
		using var reader = new StreamReader(input);

		while (!reader.EndOfStream)
		{
			this.Buffers.Add(reader.ReadLine());
		}
	}

	public int First() => GetForLength(4);

	public int Second() => GetForLength(14);

	private int GetForLength(int length)
	{
		var starts = this.Buffers.Select(Finder(length)).ToList();
		
		if (starts.Count != 1)
			Console.WriteLine(string.Join(", ", starts));

		return starts.First();
	}

	private static Func<string, int> Finder(int length) => buffer =>
	{
		var minus1 = length - 1;
		for (var i = minus1; i < buffer.Length; ++i)
		{
			if (buffer[(i - minus1)..(i + 1)].Distinct().Count() != length)
				continue;
			return i + 1;
		}

		return -1;
	};
}
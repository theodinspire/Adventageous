using Adventageous.Extensions;
using Adventageous.Utilities;

namespace Adventageous.Days;

public class Day03
{
	private readonly List<string> inventories;
	
	public Day03(Stream input)
	{
		inventories = new List<string>();
		using var reader = new StreamReader(input);

		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is null) continue;
			
			inventories.Add(line);
		}
	}

	public int First()
	{
		var foo = inventories.Select(Halve).Select(PairPriority).ToList();
		return foo.Total();
	}

	public int Second()
	{
		return this.inventories.GroupInto(3).Select(GetBadge).Select(GetPriority).Total();
	}

	private static (string Left, string Right) Halve(string s)
	{
		var half = s.Length / 2;
		return (s[..half], s[half..]);
	}

	private static int PairPriority((string Left, string Right) rucksack)
	{
		var left = new Counter<char>(rucksack.Left);
		var right = new Counter<char>(rucksack.Right);

		var intersection = left.Items.Intersect(right.Items);
		return intersection.Select(GetPriority)
			.Total();
	}

	private static int GetPriority(char item)
	{
		var value = item - 96;
		if (value > 0) return value;

		return item - 38;
	}

	private static char GetBadge(IEnumerable<string> group)
	{
		using var enumerator = group.GetEnumerator();
		if (!enumerator.MoveNext())
			throw new InvalidOperationException("Group should not be empty");
		
		var intersection = enumerator.Current.ToHashSet();

		while (enumerator.MoveNext())
		{
			intersection.IntersectWith(enumerator.Current);
		}

		if (intersection.Count != 1)
			throw new InvalidOperationException("Only one item should be common.");

		return intersection.First();
	}
}
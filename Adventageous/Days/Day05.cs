using System.Text;
using System.Text.RegularExpressions;
using Adventageous.Extensions;

namespace Adventageous.Days;

public class Day05
{
	private readonly Dictionary<int, List<char>> array = new ();
	private readonly List<Instruction> instructions = new ();

	public Day05(Stream input)
	{
		using var reader = new StreamReader(input);
		var line = reader.ReadLine();

		var binLines = new Stack<string>();

		var stackHeight = 0;
		while (line is not null && line[1] != '1')
		{
			binLines.Push(line);
			++stackHeight;
			line = reader.ReadLine();
		}
		
		if (line is null) return;
		
		const int interval = 4;
		const int intervalPosition = 1;
		const int fromEnd = interval - intervalPosition;

		var binCount = (line.Length / interval) + 1; //+1 for 1 indexed bins

		var range = Enumerable.Range(1, binCount).ToList();
		this.array = range.ToDictionary(x => x, _ => new List<char>(stackHeight));

		foreach (var binRow in binLines)
		{
			foreach (var i in range)
			{
				var index = interval * i - fromEnd;
				var item = binRow[index];

				if (item == ' ') continue;
				this.array[i].Add(item);
			}
		}

		while (true)
		{
			if (line is null)
			{
				if (reader.EndOfStream) break;
				
				line = reader.ReadLine();
				continue;
			}

			var instruction = Instruction.FromInput(line);

			if (instruction is null)
			{
				line = reader.ReadLine();
				continue;
			}
			
			this.instructions.Add(instruction.Value);
			line = reader.ReadLine();
		}
	}

	public string First()
	{
		var stacks = this.array.ToDictionary(x => x.Key, x => new Stack<char>(x.Value));
		
		foreach (var instruction in this.instructions)
		{
			var popped = stacks[instruction.From].PopMany(instruction.Count);

			stacks[instruction.To].PushMany(popped);
		}

		var sb = new StringBuilder(this.array.Count);

		foreach (var i in Enumerable.Range(1, stacks.Count))
		{
			if (!stacks[i].TryPeek(out var item)) continue;
			sb.Append(item);
		}
		
		return sb.ToString();
	}

	public string Second()
	{
		var stacks = this.array.ToDictionary(x => x.Key, x => new Stack<char>(x.Value));
		
		foreach (var instruction in this.instructions)
		{
			var popped = stacks[instruction.From].PopMany(instruction.Count);

			stacks[instruction.To].PushMany(popped.Reverse());
		}

		var sb = new StringBuilder(this.array.Count);

		foreach (var i in Enumerable.Range(1, stacks.Count))
		{
			if (!stacks[i].TryPeek(out var item)) continue;
			sb.Append(item);
		}
		
		return sb.ToString();
	}
	
	private struct Instruction
	{
		private static readonly Regex InstructionMatcher = new Regex(@"move (?<count>\d+) from (?<from>\d) to (?<to>\d)");

		private Instruction(Match match)
		{
			this.Count = int.Parse(match.Groups["count"].Value);
			this.From = int.Parse(match.Groups["from"].Value);
			this.To = int.Parse(match.Groups["to"].Value);
		}

		public static Instruction? FromInput(string input)
		{
			var match = InstructionMatcher.Match(input);
			if (!match.Success)
				return null;

			return new Instruction(match);
		}
		
		public int Count { get; }

		public int From { get; }

		public int To { get; }
	}
}
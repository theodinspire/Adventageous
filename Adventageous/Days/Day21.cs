using System.Text.RegularExpressions;
using Adventageous.Extensions;

namespace Adventageous.Days;

public partial class Day21
{
	private readonly Dictionary<string, Lazy<long>> Monkeys = new Dictionary<string, Lazy<long>>();

	public Day21(Stream input)
	{
		using var reader = new StreamReader(input);
		var parser = Parser();

		foreach (var line in reader.Lines())
		{
			var match = parser.Match(line);
			if (!match.Success) continue;

			var name = match.Groups["name"].Value;

			if (match.Groups["number"].Success)
			{
				if (long.TryParse(match.Groups["number"].Value, out var number))
					this.Monkeys.Add(name, new Lazy<long>(number));
				continue;
			}

			var left = match.Groups["left"].Value;
			var right = match.Groups["right"].Value;
			
			var op = match.Groups["operator"].Value;

			Func<long> operation = op switch
			{
				"+" => () => this.Monkeys[left].Value + this.Monkeys[right].Value,
				"-" => () => this.Monkeys[left].Value - this.Monkeys[right].Value,
				"*" => () => this.Monkeys[left].Value * this.Monkeys[right].Value,
				"/" => () => this.Monkeys[left].Value / this.Monkeys[right].Value,
				_ => throw new InvalidOperationException($"Cannot parse operation '{op}'")
			};

			this.Monkeys.Add(name, new Lazy<long>(operation));
		}
	}

	public long First()
	{
		return this.Monkeys["root"].Value;
	}

	public int Second()
	{
		return int.MinValue;
	}

	[GeneratedRegex("(?<name>\\w{4}): (?:(?<number>\\-?\\d+)|(?:(?<left>\\w{4}) (?<operator>[\\-\\+\\*\\/]) (?<right>\\w{4})))")]
	private static partial Regex Parser();
}
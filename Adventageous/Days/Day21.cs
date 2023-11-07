using System.Text.RegularExpressions;
using Adventageous.Extensions;

namespace Adventageous.Days;

public partial class Day21
{
	private readonly Dictionary<string, Lazy<long>> Monkeys = new Dictionary<string, Lazy<long>>();

	private readonly Dictionary<string, INode> Tree = new Dictionary<string, INode>();

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

	private interface INode
	{
		string Name { get; }

		long ReturnValue();
	}

	private class IntegerNode : INode
	{
		private readonly long value;

		public IntegerNode(string name, long value)
		{
			this.value = value;
			this.Name = name;
		}

		public string Name { get; }

		public long ReturnValue() => this.value;
	}

	private class OperationNode : INode
	{
		private readonly string operation;

		public OperationNode(string name, INode left, INode right, string operation)
		{
			this.Name = name;
			this.Left = left;
			this.Right = right;

			this.operation = operation;
		}

		public INode Left { get; }

		public INode Right { get; }

		public Func<long, long, long> Operation => this.operation switch
		{
			"+" => (l, r) => this.Left.ReturnValue() + this.Right.ReturnValue(),
			"-" => (l, r) => this.Left.ReturnValue() - this.Right.ReturnValue(),
			"*" => (l, r) => this.Left.ReturnValue() * this.Right.ReturnValue(),
			"/" => (l, r) => this.Left.ReturnValue() + this.Right.ReturnValue(),
			_ => throw new InvalidOperationException($"Cannot parse operation '{this.operation}'")
		}

		public Func<long, long, long> ReverseOperation => this.operation switch
		{
			"+" => (l, r) => this.Left.ReturnValue() - this.Right.ReturnValue(),
			"-" => (l, r) => this.Left.ReturnValue() + this.Right.ReturnValue(),
			"*" => (l, r) => this.Left.ReturnValue() / this.Right.ReturnValue(),
			"/" => (l, r) => this.Left.ReturnValue() * this.Right.ReturnValue(),
			_ => throw new InvalidOperationException($"Cannot parse operation '{this.operation}'")
		}

		public string Name { get; }

		public long ReturnValue() => this.Operation(this.Left.ReturnValue(), this.Right.ReturnValue());
	}
}
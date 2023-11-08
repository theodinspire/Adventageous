using System.Diagnostics;
using System.Text.RegularExpressions;
using Adventageous.Extensions;

namespace Adventageous.Days;

public partial class Day21
{
	private static string RootName = "root";

	private static string HumanName = "humn";

	private readonly Dictionary<string, INode> Tree = new Dictionary<string, INode>();

	public Day21(Stream input)
	{
		using var reader = new StreamReader(input);
		var parser = Parser();

		var seeds = new HashSet<OperationNodeSeed>();

		foreach (var line in reader.Lines())
		{
			var match = parser.Match(line);
			if (!match.Success) continue;

			var name = match.Groups["name"].Value;

			if (match.Groups["number"].Success)
			{
				if (long.TryParse(match.Groups["number"].Value, out var number))
				{
					this.Tree.Add(name, new IntegerNode(name, number));
				}
				continue;
			}

			if (!match.Groups["operator"].Success)
				continue;

			var seed = new OperationNodeSeed(name, match.Groups["left"].Value, match.Groups["right"].Value, match.Groups["operator"].Value);
			seeds.Add(seed);
		}

		while (seeds.Count > 0)
		{
			var nextLevel = seeds.Where(x => this.Tree.ContainsKey(x.LeftName) && this.Tree.ContainsKey(x.RightName)).ToHashSet();
			seeds.ExceptWith(nextLevel);

			foreach (var item in nextLevel)
			{
				var left = this.Tree[item.LeftName];
				var right = this.Tree[item.RightName];

				var node = new OperationNode(item.Name, left, right, item.Operation);
				this.Tree.Add(item.Name, node);
			}
		}
	}

	public long First()
	{
		return this.Tree[RootName].ReturnValue();
	}

	public long Second()
	{
		var r = this.Tree[RootName];
		if (r is not OperationNode root)
			throw new InvalidOperationException("Root node must be an operation");

		var x = root.Left.HasHuman ? root.Right.ReturnValue() : root.Left.ReturnValue();
		var next = root.Left.HasHuman ? root.Left : root.Right;

		while (next is OperationNode node)
		{
			next = node.Left.HasHuman ? node.Left : node.Right;
			x = node.SolveForHumanBranch(x);
		}

		return x;
	}

	[GeneratedRegex("(?<name>\\w{4}): (?:(?<number>\\-?\\d+)|(?:(?<left>\\w{4}) (?<operator>[\\-\\+\\*\\/]) (?<right>\\w{4})))")]
	private static partial Regex Parser();

	private interface INode
	{
		string Name { get; }

		bool HasHuman { get; }

		long ReturnValue();
	}

	[DebuggerDisplay("{Name}: {value}")]
	private class IntegerNode : INode
	{
		private readonly long value;

		public IntegerNode(string name, long value)
		{
			this.value = value;
			this.Name = name;

			this.HasHuman = name == HumanName;
		}

		public string Name { get; }

		public bool HasHuman { get; }

		public long ReturnValue() => this.value;
	}

	[DebuggerDisplay("{Name}: {Left.Name} {operation} {Right.Name} => {value}")]
	private class OperationNode : INode
	{
		private readonly string operation;
		private readonly long value;

		public OperationNode(string name, INode left, INode right, string operation)
		{
			this.Name = name;
			this.Left = left;
			this.Right = right;

			this.operation = operation;
			this.HasHuman = left.HasHuman || right.HasHuman;

			this.value = this.Operation(left.ReturnValue(), right.ReturnValue());
		}

		public INode Left { get; }

		public INode Right { get; }

		public Func<long, long, long> Operation => this.operation switch
		{
			"+" => (l, r) => l + r,
			"-" => (l, r) => l - r,
			"*" => (l, r) => l * r,
			"/" => (l, r) => l / r,
			_ => throw new InvalidOperationException($"Cannot parse operation '{this.operation}'")
		};

		public string Name { get; }

		public bool HasHuman { get; }

		public long ReturnValue() => this.value;

		public long SolveForHumanBranch(long x)
		{
			if (!this.HasHuman)
				throw new InvalidOperationException("Cannot solve for human, human not under this node");
			return this.Left.HasHuman ? this.SolveForLeft(x) : this.SolveForRight(x);
		}

		public long SolveForLeft(long x)
		{
			return this.operation switch
			{
				"+" => x - this.Right.ReturnValue(),
				"-" => x + this.Right.ReturnValue(),
				"*" => x / this.Right.ReturnValue(),
				"/" => x * this.Right.ReturnValue(),
				_ => throw new InvalidOperationException($"Cannot parse operation '{this.operation}'")
			};
		}

		public long SolveForRight(long x)
		{
			return this.operation switch
			{
				"+" => x - this.Left.ReturnValue(),
				"-" => this.Left.ReturnValue() - x,
				"*" => x / this.Left.ReturnValue(),
				"/" => this.Left.ReturnValue() / x,
				_ => throw new InvalidOperationException($"Cannot parse operation '{this.operation}'")
			};
		}
	}

	private readonly record struct OperationNodeSeed(string Name, string LeftName, string RightName, string Operation);
}
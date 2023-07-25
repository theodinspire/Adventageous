using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Adventageous.Extensions;

namespace Adventageous.Days;

public class Day11
{
	private readonly string input;

	public Day11(Stream input)
	{
		using var reader = new StreamReader(input);
		this.input = reader.ReadToEnd();
	}

	public long First()
	{
		var troop = Barrel.CreateTroop(input, 3);
		troop.PerformRounds(20);
		return troop.MonkeyBusiness;
	}

	public long Second()
	{
		var troop = Barrel.CreateTroop(input, 1);
		troop.PerformRounds(10_000);
		return troop.MonkeyBusiness;
	}

	private class Troop
	{
		private readonly Dictionary<int, Monkey> monkeys = new();
		private long commonModulus = 1;

		public readonly int ReliefFactor;

		public Monkey this[int index] => this.monkeys[index];

		public long MonkeyBusiness => this.monkeys.Values
			.Select(x => (long)x.TossesMade)
			.OrderDescending()
			.Take(2)
			.Aggregate(1L, (a, b) => a * b);

		public Troop(int reliefFactor)
		{
			this.ReliefFactor = reliefFactor;
		}

		public void Add(Monkey monkey)
		{
			this.monkeys.Add(monkey.Index, monkey);

			this.commonModulus *= monkey.testQuotient;

			monkey.Join(this);
		}

		public void PerformRounds(int rounds)
		{
			var ordered = this.monkeys.Values.OrderBy(x => x.Index).ToList();

			foreach (var _ in Enumerable.Range(1, rounds))
			{
				foreach (var monkey in ordered)
				{
					monkey.TossAll();
				}
			}
		}

		public long GetRelief(long inspected) =>
			(inspected / this.ReliefFactor) % this.commonModulus;
	}

	private class Monkey
	{
		public readonly int Index;

		public int TossesMade { get; private set; } = 0;

		public readonly long testQuotient;

		private readonly Queue<long> items;

		private readonly Expression<Func<long, long>> operation;

		private readonly int targetIfTrue;

		private readonly int targetIfFalse;

		private Troop troop;

		public Monkey(
			int index,
			Queue<long> items,
			Expression<Func<long, long>> operation,
			long testQuotient,
			int targetIfTrue,
			int targetIfFalse)
		{
			this.Index = index;
			this.items = items;
			this.operation = operation;
			this.testQuotient = testQuotient;
			this.targetIfTrue = targetIfTrue;
			this.targetIfFalse = targetIfFalse;
		}

		public void Join(Troop troop) => this.troop = troop;

		public void Catch(long item) => this.items.Enqueue(item);

		public void TossAll()
		{
			while (this.items.Count != 0)
			{
				this.Toss(this.items.Dequeue());
			}
		}

		public void Toss(long item)
		{
			var inspected = this.operation.Compile()(item);
			var relieved = this.troop.GetRelief(inspected);
			var target = (relieved % this.testQuotient == 0)
				? this.targetIfTrue
				: this.targetIfFalse;

			this.troop?[target].Catch(relieved);
			++(this.TossesMade);
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			// Line 1
			builder.Append("Monkey ");
			builder.Append(this.Index);
			builder.AppendLine(":");

			// Line 2
			builder.Append("  Items: ");
			builder.AppendLine(string.Join(", ", this.items));

			// Line 3
			builder.Append("  Operation: ");
			builder.AppendLine(this.operation.SimplifiedString());

			// Line 4
			builder.Append("  Test: divisible by");
			builder.AppendLine(this.testQuotient.ToString());

			// Line 5
			builder.Append("    If true: throw to monkey ");
			builder.AppendLine(this.targetIfTrue.ToString());

			// Line 6
			builder.Append("    If false: throw to monkey ");
			builder.AppendLine(this.targetIfFalse.ToString());

			return builder.ToString();
		}
	}

	private static class Barrel
	{
		private static readonly Regex MonkeyFinder = new(@"Monkey (?<index>\d+):
  Starting items: (?<items>\d+(?:, \d+)*)
  Operation: new = old (?<operator>[\+\*]) (?<operand>old|\d+)
  Test: divisible by (?<quotient>\d+)
    If true: throw to monkey (?<targetIfTrue>\d+)
    If false: throw to monkey (?<targetIfFalse>\d+)");

		public static Troop CreateTroop(string input, int reliefFactor)
		{
			var troop = new Troop(reliefFactor);

			var matches = MonkeyFinder.Matches(input);

			foreach (Match match in matches)
			{
				var monkey = CreateMonkey(match);
				if (monkey is null) continue;

				troop.Add(monkey);
			}

			return troop;
		}

		private static Monkey CreateMonkey(Match match)
		{
			if (!int.TryParse(match.Groups["index"].Value, out var index))
				return null;

			var operation = GetOperation(
				match.Groups["operator"].Value,
				match.Groups["operand"].Value);
			if (operation is null)
					return null;

			if (!long.TryParse(match.Groups["quotient"].Value, out var quotient))
				return null;

			if (!int.TryParse(match.Groups["targetIfTrue"].Value, out var targetIfTrue))
				return null;

			if (!int.TryParse(match.Groups["targetIfFalse"].Value, out var targetIfFalse))
				return null;

			var items = match.Groups["items"]
				.Value.Split(", ")
				.Select(x => long.TryParse(x, out var n) ? n : (long?)null)
				.Where(x => x is not null)
				.Cast<long>();

			return new Monkey(
				index,
				new Queue<long>(items),
				operation,
				quotient,
				targetIfTrue,
				targetIfFalse);
		}

		private static Expression<Func<long, long>> GetOperation(string operation, string operand)
		{
			return operand switch
			{
				"old" => GetReflexiveOperation(operation),
				_ => GetBinaryOperation(operation, operand)
			};
		}

		private static Expression<Func<long, long>> GetReflexiveOperation(string operation)
		{
			return operation switch
			{
				"+" => old => old + old,
				"*" => old => old * old,
				_ => null
			};
		}

		private static Expression<Func<long, long>> GetBinaryOperation(string operation, string operand)
		{
			if (!long.TryParse(operand, out var value))
			{
				return null;
			}

			return operation switch
			{
				"+" => old => old + value,
				"*" => old => old * value,
				_ => null
			};
		}
	}
}
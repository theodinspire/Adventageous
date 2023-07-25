using System.Text;

namespace Adventageous.Days;

public class Day10
{
	private const string NoOperationCode = "noop";
	private const string AdditionCode = "addx";
	private const int ScreenWidth = 40;

	private List<Instruction> instructions = new List<Instruction>();
	private bool[] screen;

	public Day10(Stream stream)
	{
		using var reader = new StreamReader(stream);

		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is null) continue;

			Instruction instruction = line[..4] switch
			{
				NoOperationCode => new NoOperation(),
				AdditionCode => new Add(int.Parse(line[5..])),
				_ => null
			};

			if (instruction is null)
				continue;

			this.instructions.Add(instruction);
		}

		var screenSize = this.instructions.Count + this.instructions.Count(x => x is Add);
		this.screen = Enumerable.Repeat(false, screenSize).ToArray();
	}

	public int First()
	{
		const int remainder = 20;

		var register = 1;
		var executionCount = 0;
		var collector = 0;

		var shouldContinue = true;
		var iterator = this.instructions.GetEnumerator();
		iterator.MoveNext();

		var position = executionCount;
		do
		{
			if (register >= position - 1 && register <= position + 1)
				this.screen[executionCount] = true;

			++executionCount;

			position = executionCount % ScreenWidth;
			if (position == remainder)
				collector += register * executionCount;

			iterator.Current?.Execute(ref register);

			if (iterator.Current?.IsFinished ?? true)
				shouldContinue = iterator.MoveNext();
		}
		while (shouldContinue);

		return collector;
	}

	public string Second()
	{
		var builder = new StringBuilder();
		builder.AppendLine();

		const int screenWidth = 40;

		for (var i = 0; i < this.screen.Length; i += screenWidth)
		{
			var line = string.Join(
				string.Empty,
				this.screen[i..(i + screenWidth)].Select(x => x ? '#' : ' '));
			builder.AppendLine(line);
		}

		return builder.ToString();
	}

	private abstract class Instruction
	{
		protected abstract int ExecutionTime { get; }

		public bool IsFinished => this.executionCount >= this.ExecutionTime;

		protected int executionCount = 0;

		public void Execute(ref int register)
		{
			this.Operate(ref register);
			++this.executionCount;
		}

		protected abstract void Operate(ref int register);
	}

	private class NoOperation : Instruction
	{
		protected override int ExecutionTime => 1;

		public NoOperation()
		{
		}

		public override string ToString() => NoOperationCode;

		protected override void Operate(ref int register)
		{
		}
	}

	private class Add : Instruction
	{
		protected override int ExecutionTime => 2;

		private readonly int amount;

		public Add(int amount)
		{
			this.amount = amount;
		}

		public override string ToString() => $"{AdditionCode} {this.amount}";

		protected override void Operate(ref int register)
		{
			if (this.executionCount == 1)
			{
				register += this.amount;
			}
		}
	}
}
using System.Collections;

namespace Adventageous.DataStructure;

public readonly struct Interval : IReadOnlyCollection<int>
{
	private readonly int start;
	private readonly int end;
	private readonly int step;

	public static Interval HalfOpen(int start, int end, int stepSize = 1) =>
		new Interval(start, end, stepSize);

	public static Interval Closed(int start, int end, int stepSize = 1)
	{
		var sign = Math.Sign(end - start);
		return new Interval(start, end + sign, stepSize);
	}

	private Interval(int start, int end, int stepSize)
	{
		if (Math.Sign(stepSize) != 1)
			throw new InvalidOperationException("Step size must be a positive integer");

		this.start = start;
		this.end = end;
		this.step = Math.Sign(this.end - this.start) * stepSize;
	}

	public int Count => (this.end - this.start) / this.step;

	public IEnumerator<int> GetEnumerator()
	{
		return new SequenceEnumerator(this.start, this.end, this.step);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	private struct SequenceEnumerator : IEnumerator<int>
	{
		private readonly int start;
		private readonly int end;
		private readonly int step;

		private readonly bool isPositive;

		public SequenceEnumerator(int start, int end, int step)
		{
			this.start = start;
			this.end = end;
			this.step = step;

			this.isPositive = Math.Sign(step) == 1;

			this.Current = start - step;
		}

		public bool MoveNext()
		{
			this.Current += this.step;

			return this.isPositive ? this.Current < this.end : this.Current > this.end;
		}

		public void Reset()
		{
			this.Current = this.start - this.step;
		}

		public int Current
		{
			get;
			private set;
		}

		object IEnumerator.Current => Current;

		public void Dispose()
		{
		}
	}
}
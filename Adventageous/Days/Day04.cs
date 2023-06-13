using System.Text.RegularExpressions;

namespace Adventageous.Days;

public class Day04
{
	private readonly List<(Interval Left, Interval Right)> intervals;

	public Day04(Stream input)
	{
		this.intervals = new List<(Interval Left, Interval Right)>();
		var reader = new StreamReader(input);

		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is null)
				continue;
			
			this.intervals.Add(Interval.FromPair(line));
		}
	}

	public int First()
	{
		return this.intervals.Count(x => x.Left.Contains(x.Right) || x.Right.Contains(x.Left));
	}

	public int Second()
	{
		return this.intervals.Count(x => x.Left.Overlaps(x.Right));
	}

	private struct Interval
	{
		private static Regex intervalMatcher = new Regex(@"(?<left>\d+)-(?<right>\d+)");
		private static Regex pairMatcher = new Regex(@"(?<one>\d+-\d+),(?<another>\d+-\d+)");
		
		public Interval(string span)
		{
			var match = intervalMatcher.Match(span);
			if (!match.Success)
				throw new InvalidOperationException("Incorrect span format");
			
			if (!int.TryParse(match.Groups["left"].Value, out var left))
				throw new InvalidOperationException("Incorrect span format");
			
			if (!int.TryParse(match.Groups["right"].Value, out var right))
				throw new InvalidOperationException("Incorrect span format");

			this.Start = Math.Min(left, right);
			this.End = Math.Max(left, right);
		}

		public int Start;

		public int End;

		public bool Contains(Interval that) =>
			this.Start <= that.Start && that.End <= this.End;

		public bool Overlaps(Interval that) =>
			this.OverlapsLeft(that) || this.OverlapsRight(that) || this.IsContainedBy(that);

		private bool OverlapsLeft(Interval that) =>
			this.Start <= that.Start && this.End >= that.Start;

		private bool OverlapsRight(Interval that) =>
			this.Start <= that.End && this.End >= that.End;

		private bool IsContainedBy(Interval that) =>
			that.Contains(this);

		public override string ToString()
		{
			return $"{this.Start}-{this.End}";
		}

		public static (Interval Left, Interval Right) FromPair(string span)
		{
			var match = pairMatcher.Match(span);
			if (!match.Success)
				throw new InvalidOperationException("Incorrect span format");

			return (new Interval(match.Groups["one"].Value),
				new Interval(match.Groups["another"].Value));
		}
	}
}
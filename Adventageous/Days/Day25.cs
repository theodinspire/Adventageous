using System.IO;
using Adventageous.Extensions;

namespace Adventageous.Days;

public class Day25
{
	private static readonly long[] Powers;
	private const long Base = 5;

	static Day25()
	{
		const int length = 20;
		Powers = new long[length];
		Powers[0] = 1;

		for (var i = 1; i < length; ++i)
		{
			Powers[i] = Powers[i - 1] * Base;
		}
	}

	private readonly List<long> numbers;

	public Day25(Stream input)
	{
		using var reader = new StreamReader(input);

		this.numbers = reader.Lines().Where(x => !string.IsNullOrWhiteSpace(x)).Select(FromSnafu).ToList();
	}

	public string First()
	{
		return ToSnafu(this.numbers.Sum());
	}

	public long Second()
	{
		return int.MinValue;
	}

	public static long FromSnafu(string value)
	{
		return value.Reverse().Select(PlaceValue).Sum();

		long PlaceValue(char digit, int place)
		{
			if (place >= Powers.Length || place < 0)
				throw new ArgumentOutOfRangeException(nameof(place), "Power not calculated for this value");
			return DigitValue(digit) * Powers[place];
		}

		int DigitValue(char digit) => digit switch
		{
			'2' => 2,
			'1' => 1,
			'0' => 0,
			'-' => -1,
			'=' => -2,
			_ => throw new ArgumentOutOfRangeException(nameof(digit), $"Cannot parse the digit '{digit}' into a value"),
		};
	}

	public static string ToSnafu(long value)
	{
		var stack = new Stack<char>();

		while (value > 0)
		{
			(value, var remainder) = long.DivRem(value, Base);
			if (remainder >= 3)
				value += 1;

			stack.Push(ToDigit(remainder));
		}

		return string.Join(string.Empty, stack);

		char ToDigit(long number) => number switch
		{
			4 => '-',
			3 => '=',
			2 => '2',
			1 => '1',
			0 => '0',
			_ => throw new ArgumentOutOfRangeException(nameof(number), $"Cannot parse digit from number {number}"),
		};
	}
}
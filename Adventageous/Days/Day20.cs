using System.Diagnostics;
using Adventageous.Extensions;
using Adventageous.Utilities;

namespace Adventageous.Days;

public class Day20
{
	private readonly List<long> values;

	public Day20(Stream input)
	{
		using var reader = new StreamReader(input);

		this.values = reader.Lines().Select(s => s?.From<long>(long.TryParse)).SelectNotNull().ToList();
	}

	public long First()
	{
		var items = this.values.Select(Item.From).ToList();
		var count = items.Count;

		var index = 0;

		while (index < count)
		{
			var item = items[index];
			if (item.Moved)
			{
				++index;
				continue;
			}

			item.Moved = true;

			if (item.Value == 0)
			{
				++index;
				continue;
			}

			var target = SomeMath.UnsignedModulo(index + item.Value, count - 1);

			items.RemoveAt(index);
			items.Insert((int)target, item);
		}

		var zeroIndex = items.FindIndex(x => x.Value == 0);

		var first = items[SomeMath.UnsignedModulo(zeroIndex + 1_000, count)].Value;
		var second = items[SomeMath.UnsignedModulo(zeroIndex + 2_000, count)].Value;
		var third = items[SomeMath.UnsignedModulo(zeroIndex + 3_000, count)].Value;

		return first + second + third;
	}

	public long Second()
	{
		const int key = 811_589_153;
		const int iterations = 10;

		var items = this.values.Select(x => x * key).Select(Item.From).ToList();
		var count = items.Count;

		for (var iteration = 0; iteration < iterations; ++iteration)
		{
			for (var index = 0; index < count; ++index)
			{
				var currentIndex = items.FindIndex(x => x.InitialIndex == index);
				var item = items[currentIndex];
				if (item.Value == 0) continue;

				var target = SomeMath.UnsignedModulo(currentIndex + item.Value, count - 1);

				items.RemoveAt(currentIndex);
				items.Insert((int)target, item);
			}
		}

		var zeroIndex = items.FindIndex(x => x.Value == 0);

		var first = items[SomeMath.UnsignedModulo(zeroIndex + 1_000, count)].Value;
		var second = items[SomeMath.UnsignedModulo(zeroIndex + 2_000, count)].Value;
		var third = items[SomeMath.UnsignedModulo(zeroIndex + 3_000, count)].Value;

		return first + second + third;
	}

	[DebuggerDisplay("Value = {Value}, Moved = {Moved}, Initial = {InitialIndex}")]
	private class Item
	{
		private Item(long value, int initialIndex)
		{
			this.Value = value;
			this.InitialIndex = initialIndex;
		}

		public long Value { get; }

		public int InitialIndex { get; }

		public bool Moved { get; set; } = false;

		public static Item From(long value, int initialIndex) => new Item(value, initialIndex);
	}
}
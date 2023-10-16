namespace Adventageous.Days;

public class Day13
{
	private List<(PacketList left, PacketList right)> pairs = new List<(PacketList, PacketList)>();

	public Day13(Stream input)
	{
		using var reader = new StreamReader(input);

		while (!reader.EndOfStream)
		{
			var left = Parse(reader.ReadLine());
			var right = Parse(reader.ReadLine());
			reader.ReadLine();

			pairs.Add((left, right));
			// Console.WriteLine(left);
			// Console.WriteLine(right);
			// Console.WriteLine();
		}
	}

	public int First()
	{
		var acc = 0;
		var i = 0;
		foreach (var (left, right) in pairs)
		{
			++i;

			if (PacketNodeComparer.Standard.Compare(left, right) <= 0)
				acc += i;
		}

		return acc;
	}

	public int Second()
	{
		const string l = "[[2]]";
		const string r = "[[6]]";

		var strings = pairs.SelectMany(x => new[] { x.left, x.right })
			.Union(new[] { Parse(l), Parse(r) })
			.Order(PacketNodeComparer.Standard)
			.Select(x => x.ToString())
			.ToList();

		var x = strings.FindIndex(s => s == l) + 1;
		var y = strings.FindIndex(s => s == r) + 1;

		return x * y;
	}

	private static PacketList Parse(string s)
	{
		var numberEnds = new[] { ',', ']' };

		var i = 1; // Assume string starts with '['
		var stack = new Stack<Queue<IPacketNode>>();
		var current = new Queue<IPacketNode>();

		while (i < s.Length - 1) // Assume string ends with ']'
		{
			var c = s[i];
			if (c == '[')
			{
				stack.Push(current);
				current = new Queue<IPacketNode>();
				i++;
				continue;
			}

			if (c == ']')
			{
				var node = new PacketList(current);
				current = stack.Pop();
				current.Enqueue(node);
				i++;
				continue;
			}

			if (!char.IsNumber(c))
			{
				i++;
				continue;
			}

			var end = s.IndexOfAny(numberEnds, i);

			if (int.TryParse(s[i..end], out var n))
			{
				current.Enqueue(new PacketNumber(n));
			}

			i = end;
		}

		return new PacketList(current);
	}

	private interface IPacketNode
	{
	}

	private class PacketList : IPacketNode
	{
		public List<IPacketNode> Items;

		public PacketList()
		{
			this.Items = new List<IPacketNode>();
		}

		public PacketList(IEnumerable<IPacketNode> nodes)
		{
			this.Items = nodes.ToList();
		}

		public PacketList(PacketNumber item)
		{
			this.Items = new List<IPacketNode> { item };
		}

		public void Add(IPacketNode node) =>this.Items.Add(node);

		public override string ToString()
		{
			return $"[{string.Join(',', this.Items)}]";
		}
	}

	private class PacketNumber : IPacketNode
	{
		public int Value;

		public PacketNumber(int value)
		{
			this.Value = value;
		}

		public PacketList Promote() => new PacketList(this);

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}

	private class PacketNodeComparer : IComparer<IPacketNode>
	{
		public static PacketNodeComparer Standard = new PacketNodeComparer();

		public int Compare(IPacketNode left, IPacketNode right)
		{
			if (ReferenceEquals(left, right))
				return 0;

			if (left is PacketNumber a && right is PacketNumber b)
				return a.Value.CompareTo(b.Value);

			if (left is PacketNumber l && right is PacketList)
				return this.Compare(l.Promote(), right);

			if (left is PacketList && right is PacketNumber r)
				return this.Compare(left, r.Promote());

			if (left is PacketList x && right is PacketList y)
				return this.CompareLists(x, y);

			return 0;
		}

		private int CompareLists(PacketList left, PacketList right)
		{
			var a = left.Items;
			var b = right.Items;
			for (var i = 0; i < Math.Min(a.Count, b.Count); ++i)
			{
				var comparison = this.Compare(a[i], b[i]);
				if (comparison != 0) return comparison;
			}

			return a.Count.CompareTo(b.Count);
		}
	}
}
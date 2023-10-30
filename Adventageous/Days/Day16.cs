using System.Text.RegularExpressions;
using Adventageous.Extensions;

namespace Adventageous.Days;

public partial class Day16
{
	private readonly Dictionary<string, Valve> Valves = new Dictionary<string, Valve>();

	public Day16(Stream input)
	{
		using var reader = new StreamReader(input);

		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is null) continue;

			var valve = Valve.Parse(line);
			if (valve is null) continue;

			this.Valves.Add(valve.Name, valve);
		}
	}

	public int First()
	{
		var open = this.Valves.Where(x => x.Value.Flow == 0)
			.Select(x => x.Key)
			.ToHashSet();
		var map = this.BuildMap()
			.Where(x => !open.Contains(x.Key.end))
			.ToDictionary(x => x.Key, x => x.Value);

		int MapValue(int timeRemaining, string current, HashSet<string> visited)
		{
			var candidates = map.Where(x => x.Key.start == current)
				.Where(x => !visited.Contains(x.Key.end));

			var maxValue = 0;

			foreach (var ((_, end), route) in candidates)
			{
				var legValue = route.Value(timeRemaining);
				if (legValue < 0) continue;

				var timeSpent = route.Distance + 1;

				visited.Add(end);
				var remainderValue = MapValue(timeRemaining - timeSpent, end, visited);
				visited.Remove(end);

				maxValue = Math.Max(remainderValue + legValue, maxValue);
			}

			return maxValue;
		}

		return MapValue(30, "AA", new HashSet<string>());
	}

	public int Second()
	{
		/*var open = this.Valves.Where(x => x.Value.Flow <= 0)
			.Select(x => x.Key)
			.ToHashSet();*/
		var needingVisiting = this.Valves.Where(x => x.Value.Flow > 0)
			.Select(x => x.Key)
			.ToHashSet();
		var map = this.BuildMap();

		int BothReady(
			int timeRemaining,
			string locationA,
			string locationB,
			HashSet<string> visited,
			int depth)
		{
			if (timeRemaining < 0) return 0;
			if (depth > needingVisiting.Count) return 0;

			var maxValue = 0;
			var toVisit = needingVisiting!.Except(visited);

			foreach (var node in toVisit)
			{
				var route = map[(locationA, node)];
				var valueA = Math.Max(route.Value(timeRemaining), 0);

				visited.Add(node);
				var valueB = OneReady(timeRemaining, locationB, node, route.Distance, visited, depth + 1);
				visited.Remove(node);

				maxValue = Math.Max(valueA + valueB, maxValue);
			}

			return maxValue;
		}

		int OneReady(
			int timeRemaining,
			string readyLocation,
			string targetedLocation,
			int timeToTarget,
			HashSet<string> visited,
			int depth)
		{
			if (timeRemaining < 0) return 0;
			if (depth > needingVisiting.Count)
				return 0;

			var maxValue = 0;
			var toVisit = needingVisiting!.Except(visited);

			foreach (var node in toVisit)
			{
				var route = map[(readyLocation, node)];
				var legValue = Math.Max(route.Value(timeRemaining), 0);
				int remainingValue;

				visited.Add(node);
				if (route.Distance == timeToTarget)
				{
					remainingValue = BothReady(
						timeRemaining - (timeToTarget + 1),
						targetedLocation,
						node,
						visited,
						depth + 1);
				}
				else if (route.Distance < timeToTarget)
				{
					remainingValue = OneReady(
						timeRemaining - (route.Distance + 1),
						node,
						targetedLocation,
						timeToTarget - (route.Distance + 1),
						visited,
						depth + 1);
				}
				else // if (route.Distance > timeToTarget)
				{
					remainingValue = OneReady(
						timeRemaining - (timeToTarget + 1),
						targetedLocation,
						node,
						route.Distance - (timeToTarget + 1),
						visited,
						depth + 1);
				}
				visited.Remove(node);

				maxValue = Math.Max(legValue + remainingValue, maxValue);
			}

			return maxValue;
		}

		return BothReady(26, "AA", "AA", new HashSet<string>(), 0);
	}

	private Dictionary<(string start, string end), Route> BuildMap()
	{
		var count = this.Valves.Count;
		var capacity = count * count;
		var map = new Dictionary<(string start, string end), Route>(capacity);

		foreach (var (name, valve) in this.Valves)
		{
			map.Add((name, name), new Route(valve));
		}

		for (var distance = 0; distance < count && map.Count < capacity; ++distance)
		{
			foreach (var (name, valve) in this.Valves)
			{
				var neighbors = valve.Neighbors;

				var nextRoutes = map.Where(x => x.Value.Distance == distance)
					.Where(x => neighbors.Contains(x.Key.start));

				foreach (var ((_, end), route) in nextRoutes.ToList())
				{
					if (map.ContainsKey((name, end))) continue;
					map.Add((name, end), new Route(valve, route));
				}
			}
		}

		if (map.Count != capacity)
			throw new InvalidOperationException("Map not fully mapped");

		return map;
	}

	private partial class Valve
	{
		private static readonly Regex Parser = BuildParser();

		public readonly string Name;
		public readonly int Flow;
		public readonly ICollection<string> Neighbors;

		private Valve(string name, int flow, ICollection<string> neighbors)
		{
			this.Name = name;
			this.Flow = flow;
			this.Neighbors = neighbors;
		}

		public static Valve? Parse(string input)
		{
			if (!Parser.TryMatch(input, out var match))
				return null;

			var name = match.Groups["name"].Value;
			var flow = int.Parse(match.Groups["flow"].Value);
			var neighbors = match.Groups["neighbors"].Value.Split(", ").ToHashSet();

			return new Valve(name, flow, neighbors);
		}

		[GeneratedRegex("Valve (?<name>\\w\\w) has flow rate=(?<flow>\\d+); tunnels? leads? to valves? (?<neighbors>(?:\\w\\w, )*\\w\\w)")]
		private static partial Regex BuildParser();
	}

	private class Route
	{
		public readonly Valve Start;
		public readonly Valve End;

		public readonly int Distance;
		public readonly Valve? Next;

		public Route(Valve self)
		{
			this.Start = self;
			this.End = self;
			this.Distance = 0;
			this.Next = null;
		}

		public Route(Valve start, Route neighboring)
		{
			this.Start = start;
			this.End = neighboring.End;
			this.Distance = neighboring.Distance + 1;
			this.Next = neighboring.Start;
		}

		public int Value(int timeRemaining) =>
			(timeRemaining - this.Distance - 1) * this.End.Flow;
	}
}
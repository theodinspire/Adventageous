using System.Text.RegularExpressions;
using Adventageous.DataStructure;

namespace Adventageous.Days;

public partial class Day15
{
	private readonly List<Sensor> Sensors = new List<Sensor>();
	private readonly ISet<Point> Beacons;

	public Day15(Stream input)
	{
		using var reader = new StreamReader(input);

		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is null) continue;

			var sensor = Sensor.Parse(line);
			if (sensor is null) continue;

			this.Sensors.Add(sensor);
		}

		this.Beacons = this.Sensors.Select(x => x.ClosestBeacon).ToHashSet();
	}

	public int First(int row)
	{
		var xes = this.Sensors.SelectMany(x => new[] { x.Location.X, x.ClosestBeacon.X }).ToHashSet();
		var maxDistance = this.Sensors.Select(x => x.BeaconRadius).Max();

		var left = xes.Min() - maxDistance;
		var right = xes.Max() + maxDistance;

		return Interval.Closed(left, right)
			.Select(x => new Point(x, row))
			.Where(x => !Beacons.Contains(x))
			.Count(p => this.Sensors.Any(s => s.WithinBeaconDistance(p)));
	}

	public int Second(int maxDimension)
	{



		var point = Point.Origin;
		return (point.X * 4_000_000) + point.Y;
	}

	private partial class Sensor
	{
		private static readonly Regex Pattern = LinePattern();

		public readonly Point Location;
		public readonly Point ClosestBeacon;

		public readonly int BeaconRadius;

		public Sensor(Point location, Point closestBeacon)
		{
			this.Location = location;
			this.ClosestBeacon = closestBeacon;

			this.BeaconRadius = this.Location.ManhattanDistance(this.ClosestBeacon);
		}

		public static Sensor? Parse(string line)
		{
			var match = Pattern.Match(line);
			if (!match.Success)
				return null;

			if (!int.TryParse(match.Groups["locationX"].Value, out var locationX))
				return null;
			if (!int.TryParse(match.Groups["locationY"].Value, out var locationY))
				return null;
			if (!int.TryParse(match.Groups["beaconX"].Value, out var beaconX))
				return null;
			if (!int.TryParse(match.Groups["beaconY"].Value, out var beaconY))
				return null;

			return new Sensor((locationX, locationY), (beaconX, beaconY));
		}

		public bool WithinBeaconDistance(Point point)
		{
			return point.ManhattanDistance(this.Location) <= this.BeaconRadius;
		}

		[GeneratedRegex("Sensor at x=(?<locationX>\\-?\\d+), y=(?<locationY>\\-?\\d+): closest beacon is at x=(?<beaconX>\\-?\\d+), y=(?<beaconY>\\-?\\d+)")]
		private static partial Regex LinePattern();
	}
}
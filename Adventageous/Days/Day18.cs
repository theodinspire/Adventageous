using Adventageous.DataStructure;

namespace Adventageous.Days;

public class Day18
{
	private readonly HashSet<Point3> lavaCubes = new HashSet<Point3>();

	private readonly int minX;
	private readonly int minY;
	private readonly int minZ;
	private readonly int maxX;
	private readonly int maxY;
	private readonly int maxZ;

	public Day18(Stream input)
	{
		using var reader = new StreamReader(input);

		var minX = int.MaxValue;
		var minY = int.MaxValue;
		var minZ = int.MaxValue;
		var maxX = int.MinValue;
		var maxY = int.MinValue;
		var maxZ = int.MinValue;

		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is null)
				continue;

			var point = Point3.Parse(line);

			this.lavaCubes.Add(point);

			minX = Math.Min(minX, point.X);
			minY = Math.Min(minY, point.Y);
			minZ = Math.Min(minZ, point.Z);
			maxX = Math.Max(maxX, point.X);
			maxY = Math.Max(maxY, point.Y);
			maxZ = Math.Max(maxZ, point.Z);
		}

		this.minX = minX;
		this.minY = minY;
		this.minZ = minZ;
		this.maxX = maxX;
		this.maxY = maxY;
		this.maxZ = maxZ;
	}

	public int First()
	{
		return this.lavaCubes.Select(LavaNeighbors).Select(x => 6 - x).Sum();

		int LavaNeighbors(Point3 point) => point.TaxiCabNeighbors.Count(this.lavaCubes.Contains);
	}

	public int Second()
	{
		var min = new Point3(this.minX - 1, this.minY - 1, this.minZ - 1);
		var max = new Point3(this.maxX + 1, this.maxY + 1, this.maxZ + 1);

		var queue = new Queue<Point3>();
			queue.Enqueue(min);
		var exterior = new HashSet<Point3> { min };

		while (queue.Count > 0)
		{
			var item = queue.Dequeue();
			var next = item.TaxiCabNeighbors
				.Where(p => !exterior.Contains(p)) // Already accounted for
				.Where(p => !this.lavaCubes.Contains(p)) // In main volume
				.Where(InBounds);

			foreach (var point in next)
			{
				exterior.Add(point);
				queue.Enqueue(point);
			}
		}

		var length = max.X - min.X + 1;
		var depth = max.Y - min.Y + 1;
		var height = max.Z - min.Z + 1;

		var exteriorArea = 2 * (length * depth + depth * height + height * length);

		var surfaceArea = exterior.Select(ExteriorNeighbors).Select(x => 6 - x).Sum();

		return surfaceArea - exteriorArea;

		bool InBounds(Point3 p) =>
				p.X >= min.X && p.X <= max.X &&
				p.Y >= min.Y && p.Y <= max.Y &&
				p.Z >= min.Z && p.Z <= max.Z;

		int ExteriorNeighbors(Point3 point) => point.TaxiCabNeighbors.Count(exterior!.Contains);
	}
}
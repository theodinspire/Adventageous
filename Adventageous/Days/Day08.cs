namespace Adventageous.Days;

public class Day08
{
	private int[][] trees;

	private int Width => this.trees[0].Length;

	private int Height => this.trees.Length;
	
	public Day08(Stream input)
	{
		var lines = new List<string>();
		
		using var reader = new StreamReader(input);

		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is null || line.Length == 0)
				continue;
			
			lines.Add(line);
		}

		this.trees = lines.Select(
				l => l.Select(c => (int)c - 48).ToArray()) // (int)'0' == 48
			.ToArray();
	}

	public int First()
	{
		var height = this.Height;
		var width = this.Width;

		var visible = 2 * height + 2 * width - 4;

		for (var row = 1; row < height - 1; ++row)
		{
			for (var column = 1; column < width - 1; ++column)
			{
				var tree = this.trees[row][column];
				bool Shorter(int t) => t < tree;

				var left = this.trees[row][..column];
				var right = this.trees[row][(column + 1)..];
				var up = this.trees[..row].Select(r => r[column]);
				var down = this.trees[(row + 1)..].Select(r => r[column]);

				var isVisible = left.All(Shorter) || right.All(Shorter) || up.All(Shorter) || down.All(Shorter);
				if (isVisible)
					++visible;
			}
		}
		
		return visible;
	}

	public int Second()
	{var height = this.Height;
		var width = this.Width;

		var maxScore = 1;

		for (var row = 1; row < height - 1; ++row)
		{
			for (var column = 1; column < width - 1; ++column)
			{
				var tree = this.trees[row][column];
				bool Shorter(int t) => t < tree;

				var left = this.trees[row][..column].Reverse().TakeWhile(Shorter).Count();
				var right = this.trees[row][(column + 1)..].TakeWhile(Shorter).Count();
				var up = this.trees[..row].Select(r => r[column]).Reverse().TakeWhile(Shorter).Count();
				var down = this.trees[(row + 1)..].Select(r => r[column]).TakeWhile(Shorter).Count();

				var score =
					(left  + (left               < column ? 1 : 0)) *
					(right + (column + right + 1 < width  ? 1 : 0)) *
					(up    + (up                 < row    ? 1 : 0)) * 
					(down  + (row + down + 1     < height ? 1 : 0));
				if (score > maxScore)
					maxScore = score;
			}
		}

		return maxScore;
	}
}
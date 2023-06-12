using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Adventageous.Days
{
	public class Day01
	{
		private readonly List<List<int>> inventories = new List<List<int>>();

		public Day01(Stream input)
		{
			using var reader = new StreamReader(input);

			do
			{
				var inventory = new List<int>();

				do
				{
					var line = reader.ReadLine();

					if (!int.TryParse(line, out var value))
						break;
					
					inventory.Add(value);
				}
				while (!reader.EndOfStream);
			}
			while (!reader.EndOfStream);
		}

		private static int Sum(ICollection<int> collection)
		{
			return collection.Aggregate(0, (x, y) => x + y);
		}
	}
}
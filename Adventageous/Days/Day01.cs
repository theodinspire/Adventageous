using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Adventageous.Extensions;

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
				
				inventories.Add(inventory);
			}
			while (!reader.EndOfStream);
		}

		public int First()
		{
			return inventories.Select(x => x.Sum()).Max();
		}

		public int Second()
		{
			return inventories.Select(x => x.Sum())
				.OrderDescending()
				.Take(3)
				.Sum();
		}
	}
}
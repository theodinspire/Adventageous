using System.Text.RegularExpressions;
using Adventageous.Extensions;

namespace Adventageous.Days;

public partial class Day19
{
	private readonly List<Blueprint> blueprints = new List<Blueprint>();

	public Day19(Stream input)
	{
		using var reader = new StreamReader(input);

		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is null) continue;

			var blueprint = Blueprint.Parse(line);
			if (blueprint is null) continue;

			this.blueprints.Add(blueprint);
		}
	}

	public int First()
	{
		return this.blueprints.Select(b => b.Id * GetMaximalGeodeYield(b, 24)).Sum();
	}

	public int Second()
	{
		return this.blueprints.Take(3).Select(b => GetMaximalGeodeYield(b, 32)).Product();
	}

	private static int GetMaximalGeodeYield(Blueprint blueprint, int startingRounds)
	{
		var initialState = new InitialState
		{
			RoundsRemaining = startingRounds,
			ProductionRate = new Material { Ore = 1 }
		};

		var queue = new Queue<InitialState>();
		queue.Enqueue(initialState);

		var maxYield = 0;
		var iteration = 0;

		while (queue.Count > 0)
		{
			var state = queue.Dequeue();
			if (state.RoundsRemaining <= 0) continue;

			maxYield = int.Max(maxYield, state.TrivialGeodeYield);

			EnqueueOreBot(queue, blueprint, state);
			EnqueueClayBot(queue, blueprint, state);
			EnqueueObsidianBot(queue, blueprint, state);
			EnqueueGeodeBot(queue, blueprint, state);
		}

		return maxYield;
	}

	private static void EnqueueOreBot(Queue<InitialState> queue, Blueprint blueprint, InitialState currentState)
	{
		var maxNeed = int.Max(
			int.Max(blueprint.OreBotCost.Ore, blueprint.ClayBotCost.Ore),
			int.Max(blueprint.ObsidianBotCost.Ore, blueprint.GeodeBotCost.Ore));
		if (currentState.ProductionRate.Ore * currentState.RoundsRemaining + currentState.Inventory.Ore >=
		    maxNeed * currentState.RoundsRemaining)
			return;

		var oreBotRounds = currentState.RoundsUntilOreBot(blueprint) + 1;
		if (oreBotRounds >= currentState.RoundsRemaining)
			return;

		var roundsRemaining = currentState.RoundsRemaining - oreBotRounds;
		var inventory = currentState.Inventory + (oreBotRounds * currentState.ProductionRate) - blueprint.OreBotCost;
		var productionRate = currentState.ProductionRate + new Material { Ore = 1 };

		var newState = new InitialState
		{
			RoundsRemaining = roundsRemaining,
			Inventory = inventory,
			ProductionRate = productionRate,
		};
		queue.Enqueue(newState);
	}

	private static void EnqueueClayBot(Queue<InitialState> queue, Blueprint blueprint, InitialState currentState)
	{
		if (currentState.ProductionRate.Clay * currentState.RoundsRemaining + currentState.Inventory.Clay >=
		    blueprint.ObsidianBotCost.Clay * currentState.RoundsRemaining)
			return;

		var clayBotRounds = currentState.RoundsUntilClayBot(blueprint) + 1;
		if (clayBotRounds >= currentState.RoundsRemaining)
			return;

		var inventory = currentState.Inventory + (clayBotRounds * currentState.ProductionRate) - blueprint.ClayBotCost;
		var productionRate = currentState.ProductionRate + new Material { Clay = 1 };

		var newState = new InitialState
		{
			RoundsRemaining = currentState.RoundsRemaining - clayBotRounds,
			ProductionRate = productionRate,
			Inventory = inventory
		};
		queue.Enqueue(newState);
	}

	private static void EnqueueObsidianBot(Queue<InitialState> queue, Blueprint blueprint, InitialState currentState)
	{
		if (currentState.ProductionRate.Obsidian * currentState.RoundsRemaining + currentState.Inventory.Obsidian >=
		    blueprint.GeodeBotCost.Obsidian * currentState.RoundsRemaining)
			return;

		var obsidianBotRounds = currentState.RoundsUntilObsidianBot(blueprint) + 1;
		if (obsidianBotRounds >= currentState.RoundsRemaining)
			return;

		var inventory = currentState.Inventory + (obsidianBotRounds * currentState.ProductionRate) - blueprint.ObsidianBotCost;
		var productionRate = currentState.ProductionRate + new Material { Obsidian = 1 };

		var newState = new InitialState
		{
			RoundsRemaining = currentState.RoundsRemaining - obsidianBotRounds,
			ProductionRate = productionRate,
			Inventory = inventory
		};
		queue.Enqueue(newState);
	}

	private static void EnqueueGeodeBot(Queue<InitialState> queue, Blueprint blueprint, InitialState currentState)
	{
		var geodeBotRounds = currentState.RoundsUntilGeodeBot(blueprint) + 1;
		if (geodeBotRounds >= currentState.RoundsRemaining)
			return;

		var inventory = currentState.Inventory + (geodeBotRounds * currentState.ProductionRate) - blueprint.GeodeBotCost;
		var productionRate = currentState.ProductionRate + new Material { Geode = 1 };

		var newState = new InitialState
		{
			RoundsRemaining = currentState.RoundsRemaining - geodeBotRounds,
			ProductionRate = productionRate,
			Inventory = inventory
		};
		queue.Enqueue(newState);
	}

	private partial class Blueprint
	{
		public int Id { get; init; }

		public Material OreBotCost { get; init; }

		public Material ClayBotCost { get; init; }

		public Material ObsidianBotCost { get; init; }

		public Material GeodeBotCost { get; init; }

		private Blueprint() {}

		public static Blueprint? Parse(string input)
		{
			var match = BlueprintMatcher().Match(input);

			if (!match.Success) return null;

			if (!int.TryParse(match.Groups["id"].Value, out var id))
				return null;

			if (!int.TryParse(match.Groups["oreOre"].Value, out var oreOre))
				return null;

			if (!int.TryParse(match.Groups["clayOre"].Value, out var clayOre))
				return null;

			if (!int.TryParse(match.Groups["obsidianOre"].Value, out var obsidianOre))
				return null;

			if (!int.TryParse(match.Groups["obsidianClay"].Value, out var obsidianClay))
				return null;

			if (!int.TryParse(match.Groups["geodeOre"].Value, out var geodeOre))
				return null;

			if (!int.TryParse(match.Groups["geodeObsidian"].Value, out var geodeObsidian))
				return null;

			return new Blueprint
			{
				Id = id,
				OreBotCost = new Material { Ore = oreOre },
				ClayBotCost = new Material { Ore = clayOre },
				ObsidianBotCost = new Material { Ore = obsidianOre, Clay = obsidianClay },
				GeodeBotCost = new Material { Ore = geodeOre, Obsidian = geodeObsidian },
			};
		}

		[GeneratedRegex("Blueprint (?<id>\\d+): Each ore robot costs (?<oreOre>\\d+) ore. Each clay robot costs (?<clayOre>\\d+) ore. Each obsidian robot costs (?<obsidianOre>\\d+) ore and (?<obsidianClay>\\d+) clay. Each geode robot costs (?<geodeOre>\\d+) ore and (?<geodeObsidian>\\d+) obsidian.")]
		private static partial Regex BlueprintMatcher();
	}

	private readonly struct InitialState
	{
		public int RoundsRemaining { get; init; }
		
		public Material ProductionRate { get; init; }

		public Material Inventory { get; init; }

		public int TrivialGeodeYield =>
			this.Inventory.Geode + this.RoundsRemaining * this.ProductionRate.Geode;

		public int RoundsUntilOreBot(Blueprint blueprint) =>
			this.RoundsUntilGivenBoxForGivenMaterial(blueprint, b => b.OreBotCost, m => m.Ore);

		public int RoundsUntilClayBot(Blueprint blueprint) =>
			this.RoundsUntilGivenBoxForGivenMaterial(blueprint, b => b.ClayBotCost, m => m.Ore);

		public int RoundsUntilObsidianBot(Blueprint blueprint)
		{
			Material BotSelector(Blueprint b) => b.ObsidianBotCost;
			var ore = this.RoundsUntilGivenBoxForGivenMaterial(blueprint, BotSelector, m => m.Ore);
			var clay = this.RoundsUntilGivenBoxForGivenMaterial(blueprint, BotSelector, m => m.Clay);

			return int.Max(ore, clay);
		}

		public int RoundsUntilGeodeBot(Blueprint blueprint)
		{
			Material BotSelector(Blueprint b) => b.GeodeBotCost;
			var ore = this.RoundsUntilGivenBoxForGivenMaterial(blueprint, BotSelector, m => m.Ore);
			var obsidian = this.RoundsUntilGivenBoxForGivenMaterial(blueprint, BotSelector, m => m.Obsidian);

			return int.Max(ore, obsidian);
		}

		private int RoundsUntilGivenBoxForGivenMaterial(Blueprint blueprint, Func<Blueprint, Material> botSelector, Func<Material, int> materialSelector)
		{
			const int maxRounds = int.MaxValue - 100;

			var bluePrintBotCost = materialSelector(botSelector(blueprint));
			var inventoryMaterial = materialSelector(this.Inventory);
			var productionRate = materialSelector(this.ProductionRate);

			if (inventoryMaterial >= bluePrintBotCost) return 0;
			if (productionRate <= 0) return maxRounds;

			var (quotient, remainder) = Math.DivRem((bluePrintBotCost - inventoryMaterial), productionRate);
			return quotient + int.Sign(remainder);
		}
	}

	private readonly struct Material
	{
		public int Ore { get; init; }

		public int Clay { get; init; }

		public int Obsidian { get; init; }

		public int Geode { get; init; }

		public Material() {}

		public Material(int ore, int clay, int obsidian, int geode)
		{
			this.Ore = ore;
			this.Clay = clay;
			this.Obsidian = obsidian;
			this.Geode = geode;
		}

		public static Material operator +(Material left, Material right) =>
			new Material(left.Ore + right.Ore, left.Clay + right.Clay, left.Obsidian + right.Obsidian, left.Geode + right.Geode);

		public static Material operator -(Material self) =>
			new Material(-self.Ore, -self.Clay, -self.Obsidian, -self.Geode);

		public static Material operator -(Material left, Material right) => left + -right;

		public static Material operator *(int scalar, Material right) =>
			new Material(scalar * right.Ore, scalar * right.Clay, scalar * right.Obsidian, scalar * right.Geode);

		public bool CanAfford(Material that) =>
			(this - that) is { Ore: >= 0, Clay: >= 0, Obsidian: >= 0 };
	}
}
using System.Text.RegularExpressions;
using Adventageous.Extensions;

namespace Adventageous.Days;

public class Day02
{
	private static readonly Regex RoundMatcher = new Regex(@"(?<their>[ABC]) (?<mine>[XYZ])");
	
	private List<Match> Rounds;

	public Day02(Stream input)
	{
		this.Rounds = new List<Match>();
		using var reader = new StreamReader(input);

		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is null) return;
			
			var match = RoundMatcher.Match(line);
			if (!match.Success) continue;
			
			this.Rounds.Add(match);
		}
	}

	public int First()
	{
		return this.Rounds.Select(ScoreRoundFirst).Sum();
	}

	public int Second()
	{
		return this.Rounds.Select(ScoreRoundSecond).Sum();
	}

	private static Outcome GetOutcome((Hand their, Hand me) round)
	{
		switch (round)
		{
			case (Hand.Paper, Hand.Scissors):
			case (Hand.Scissors, Hand.Rock):
			case (Hand.Rock, Hand.Paper):
				return Outcome.Win;
			case (Hand.Paper, Hand.Rock):
			case (Hand.Scissors, Hand.Paper):
			case (Hand.Rock, Hand.Scissors):
				return Outcome.Lose;
			default:
				return Outcome.Draw;
		}
	}

	private static int ScoreRoundFirst(Match round)
	{
		var pair = ProcessRoundFirst(round);
		return (int)GetOutcome(pair) + (int)pair.me;
	}

	private static int ScoreRoundSecond(Match round)
	{
		var pair = ProcessRoundSecond(round);
		return (int)pair.outcome + (int)MyHandSecond(pair);
	}

	private static (Hand their, Hand me) ProcessRoundFirst(Match round)
	{
		return (TheirHand(round.Groups["their"].Value), MyHandFirst(round.Groups["mine"].Value));
	}

	private static (Hand their, Outcome outcome) ProcessRoundSecond(Match round)
	{
		return (TheirHand(round.Groups["their"].Value), GetOutcomeSecond(round.Groups["mine"].Value));
	}

	private static Hand TheirHand(string symbol)
	{
		return symbol switch
		{
			"A" => Hand.Rock,
			"B" => Hand.Paper,
			"C" => Hand.Scissors,
			_ => Hand.None
		};
	}

	private static Hand MyHandFirst(string symbol)
	{
		return symbol switch
		{
			"X" => Hand.Rock,
			"Y" => Hand.Paper,
			"Z" => Hand.Scissors,
			_ => Hand.None
		};
	}

	private static Outcome GetOutcomeSecond(string symbol)
	{
		return symbol switch
		{
			"X" => Outcome.Lose,
			"Y" => Outcome.Draw,
			"Z" => Outcome.Win,
			_ => Outcome.None
		};
	}

	private static Hand MyHandSecond((Hand their, Outcome outcome) round)
	{
		switch (round)
		{
			// Rock
			case (Hand.Scissors, Outcome.Win):
			case (Hand.Rock, Outcome.Draw):
			case (Hand.Paper, Outcome.Lose):
				return Hand.Rock;
			// Paper
			case (Hand.Rock, Outcome.Win):
			case (Hand.Paper, Outcome.Draw):
			case (Hand.Scissors, Outcome.Lose):
					return Hand.Paper;
			// Scissors
			case (Hand.Paper, Outcome.Win):
			case (Hand.Scissors, Outcome.Draw):
			case (Hand.Rock, Outcome.Lose):
				return Hand.Scissors;
			// ¯\_(ツ)_/¯
			default:
				return Hand.None;
		}
	}

	private enum Hand
	{
		None,
		Rock = 1,
		Paper = 2,
		Scissors = 3,
	}

	private enum Outcome
	{
		None = int.MinValue,
		Lose = 0,
		Draw = 3,
		Win = 6,
	}
}
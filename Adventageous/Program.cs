using Adventageous.Days;
var file =
	// File.OpenRead(@"Data\Example\02a");
	File.OpenRead(@"Data\Actual\02");

var day = new Day02(file);

Console.WriteLine($"1st: {day.First()}");
Console.WriteLine($"2nd: {day.Second()}");
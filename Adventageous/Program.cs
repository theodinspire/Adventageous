using Adventageous.Days;
var file =
	// File.OpenRead(@"Data\Example\05a");
	File.OpenRead(@"Data\Actual\05");

var day = new Day05(file);

Console.WriteLine($"1st: {day.First()}");
Console.WriteLine($"2nd: {day.Second()}");